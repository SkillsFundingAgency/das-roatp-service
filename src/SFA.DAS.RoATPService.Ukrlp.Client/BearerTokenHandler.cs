using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Ukrlp.Client;

[ExcludeFromCodeCoverage]
public sealed class BearerTokenHandler(IOAuthTokenService _tokenService, UkrlpApiAuthentication _options) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenService.GetAccessTokenAsync(cancellationToken);

        // Set Accept header if not already present
        if (!request.Headers.Contains("Accept"))
        {
            request.Headers.Add("Accept", "application/x-ndjson");
        }

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
