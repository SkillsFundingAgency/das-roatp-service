using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.RoATPService.Ukrlp.Client;

public interface IOAuthTokenService
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
    void InvalidateAccessToken();
}

public sealed class OAuthTokenService : IOAuthTokenService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly UkrlpApiAuthentication _config;
    private string _cachedToken;
    private DateTimeOffset _tokenExpiry = DateTimeOffset.MinValue;
    private Task<string> _tokenTask;
    private readonly Object _sync = new();
    private readonly ILogger<OAuthTokenService> _logger;

    // Inject IHttpClientFactory instead of HttpClient directly
    public OAuthTokenService(IHttpClientFactory httpClientFactory, UkrlpApiAuthentication config, ILogger<OAuthTokenService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _config = config;
    }


    public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedToken is not null && DateTimeOffset.UtcNow < _tokenExpiry)
            return Task.FromResult(_cachedToken);

        lock (_sync)
        {
            // Double-check after acquiring the lock
            if (_cachedToken is not null && DateTimeOffset.UtcNow < _tokenExpiry)
                return Task.FromResult(_cachedToken);

            // If a token request is already in progress, reuse it
            if (_tokenTask is not null)
                return _tokenTask;

            // Start a new token request. Use CancellationToken.None for the shared fetch
            // so a caller cancelling their token won't cancel the shared in-flight request.
            _tokenTask = FetchAndCacheToken(CancellationToken.None);
            return _tokenTask;
        }
    }

    private async Task<string> FetchAndCacheToken(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching new token for Ukrlp Api");
            using var client = _httpClientFactory.CreateClient(Constants.TokenClientName);

            var body = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _config.ClientId,
                ["client_secret"] = _config.ClientSecret,
                ["scope"] = _config.Scope,
            });

            var response = await client.PostAsync(_config.TokenEndpoint, body, cancellationToken);
            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken)
                ?? throw new InvalidOperationException("Empty token response.");

            lock (_sync)
            {
                // Subtract a 30s buffer to refresh slightly before expiry. Ensure we don't
                // set an expiry in the past if the token lifetime is very small.
                var bufferSeconds = 30;
                var effectiveExpirySeconds = Math.Max(5, tokenResponse.ExpiresIn - bufferSeconds);
                _cachedToken = tokenResponse.AccessToken;
                _tokenExpiry = DateTimeOffset.UtcNow.AddSeconds(effectiveExpirySeconds);
                // Clear the in-flight task reference
                _tokenTask = null;
            }

            return _cachedToken;
        }
        catch
        {
            // Clear the in-flight task reference on failure to allow retries
            lock (_sync)
            {
                _tokenTask = null;
            }
            throw;
        }
    }

    public void InvalidateAccessToken()
    {
        lock (_sync)
        {
            _cachedToken = null;
            _tokenExpiry = DateTimeOffset.MinValue;
            _tokenTask = null;
        }
    }
}

public record TokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_in")] int ExpiresIn,
    [property: JsonPropertyName("token_type")] string TokenType
);
