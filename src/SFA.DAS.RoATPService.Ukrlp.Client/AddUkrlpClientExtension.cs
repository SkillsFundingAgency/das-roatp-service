using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace SFA.DAS.RoATPService.Ukrlp.Client;

[ExcludeFromCodeCoverage]
public static class AddUkrlpClientExtension
{
    public static void AddUkrlpClient(this IServiceCollection services, UkrlpApiAuthentication ukrlpConfig)
    {
        services.AddSingleton<IOAuthTokenService, OAuthTokenService>();
        services.AddHttpClient(Constants.TokenClientName, c => c.BaseAddress = new Uri(ukrlpConfig.TokenEndpoint));
        services.AddTransient<BearerTokenHandler>();
        services
            .AddHttpClient<IUkrlpService, UkrlpService>(c => c.BaseAddress = new Uri(ukrlpConfig.ApiBaseAddress))
            .AddHttpMessageHandler<BearerTokenHandler>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression =
                    DecompressionMethods.Brotli |
                    DecompressionMethods.GZip |
                    DecompressionMethods.Deflate
            })
            .AddPolicyHandler((serviceProvider, request) =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<UkrlpService>>();
                return HttpPolicyExtensions
                    .HandleTransientHttpError() // 5xx + network errors
                    .WaitAndRetryAsync([TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4)],
                    (outcome, timespan, retryAttempt, context) =>
                    {
                        logger.LogWarning("Error retrieving response from UKRLP. Reason: {ErrorMessage}. Retrying in {Seconds} secs...attempt: {RetryAttempt}", outcome.Exception.Message, timespan.Seconds, retryAttempt);
                    });
            });
    }
}
