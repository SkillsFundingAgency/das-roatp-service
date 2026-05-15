using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.RoATPService.Ukrlp.Client;

public interface IOAuthTokenService
{
    Task<string> GetAccessTokenAsync(CancellationToken ct = default);
    void InvalidateAccessToken();
}

public sealed class OAuthTokenService : IOAuthTokenService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly UkrlpApiAuthentication _options;
    private string _cachedToken;
    private DateTimeOffset _tokenExpiry = DateTimeOffset.MinValue;
    private Task<string> _tokenTask;
    private readonly Object _sync = new();
    private readonly ILogger<OAuthTokenService> _logger;

    // Inject IHttpClientFactory instead of HttpClient directly
    public OAuthTokenService(IHttpClientFactory httpClientFactory, UkrlpApiAuthentication options, ILogger<OAuthTokenService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _options = options;
    }


    public Task<string> GetAccessTokenAsync(CancellationToken ct = default)
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

    private async Task<string> FetchAndCacheToken(CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("Fetching new token for Ukrlp Api");
            using var client = _httpClientFactory.CreateClient("token-client");

            var body = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret,
                ["scope"] = _options.Scope,
            });

            var response = await client.PostAsync(_options.TokenEndpoint, body, ct);
            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(ct)
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

public sealed class BearerTokenHandler(IOAuthTokenService _tokenService, UkrlpApiAuthentication _options) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenService.GetAccessTokenAsync(cancellationToken);

        // Set Accept header if not already present
        //if (!request.Headers.Contains("Accept"))
        //{
        //    request.Headers.Add("Accept", "application/x-ndjson");
        //}

        //// Add other headers only if missing to avoid duplicate header values when retrying
        //if (!request.Headers.Contains("Accept-Encoding"))
        //{
        //    request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
        //}

        if (!request.Headers.Contains("Ocp-Apim-Subscription-Key"))
        {
            request.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", _options.SubscriptionKey);
        }

        // Always set/overwrite Authorization header
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await base.SendAsync(request, cancellationToken);

        // If 401, invalidate cache and retry once with a fresh token
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            response.Dispose();

            _tokenService.InvalidateAccessToken();

            token = await _tokenService.GetAccessTokenAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }
}

public interface IUkrlpService
{
    Task<UkrlpResponse> GetProviderDataAsync(IEnumerable<int> ukprn, CancellationToken ct = default);
}

public class UkrlpService(HttpClient _httpClient) : IUkrlpService
{
    // Example method to call UKRLP API
    public async Task<UkrlpResponse> GetProviderDataAsync(IEnumerable<int> ukprn, CancellationToken ct = default)
    {
        var ukprns = string.Join("&ukprns=", ukprn);
        var response = await _httpClient.GetAsync($"api/providers?ukprns={ukprns}", ct);
        response.EnsureSuccessStatusCode();
        var str = await response.Content.ReadFromJsonAsync<JsonObject>(ct);
        return null;

        return await response.Content.ReadFromJsonAsync<UkrlpResponse>(ct);
    }
}

public record UkrlpResponse(IEnumerable<Provider> MatchingProviderRecords);
public class Provider
{
    public string UKPRN { get; set; }
    public string ProviderName { get; set; }
    public string ProviderStatus { get; set; }
    public DateTime VerificationDate { get; set; }
    public Address LegalAddress { get; set; }
}

public record Address(string Address1, string Address2, string Address3, string Address4, string Town, string City, string PostCode);
