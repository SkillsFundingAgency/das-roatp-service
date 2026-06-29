using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.RoATPService.Ukrlp.SoapClient;

namespace SFA.DAS.RoATPService.Ukrlp.Client.SoapClient;

[ExcludeFromCodeCoverage]
public static class AddUkrlpSoapServiceExtension
{
    public static IServiceCollection AddUkrlpSoapService(this IServiceCollection services)
    {
        UkrlpSoapApiAuthentication ukrlpApiAuthentication = new()
        {
            ApiBaseAddress = "http://webservices.ukrlp.co.uk/UkrlpProviderQueryWS6/ProviderQueryServiceV6",
            QueryId = "2",
            StakeholderId = "2"
        };
        services.AddSingleton(ukrlpApiAuthentication);
        services.AddHttpClient<IUkrlpSoapApiClient, UkrlpSoapApiClient>();
        services.AddSingleton<IUkrlpSoapSerializer, UkrlpSoapSerializer>();

        return services;
    }
}
