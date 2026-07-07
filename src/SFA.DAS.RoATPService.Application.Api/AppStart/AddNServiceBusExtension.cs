using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using static SFA.DAS.RoATPService.Application.Api.AppStart.ConfigureNServiceBusExtension;

namespace SFA.DAS.RoATPService.Application.Api.AppStart;

[ExcludeFromCodeCoverage]
public static class AddNServiceBusExtension
{
    public static IServiceCollection AddNServiceBus(this IServiceCollection services, IConfiguration configuration)
    {
        var endpointConfiguration = new EndpointConfiguration("SFA.DAS.RoATPService");

        var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
        transport.Transport.SubscriptionRuleNamingConvention = AzureRuleNameShortener.Shorten;
        transport.ConnectionString(configuration["AzureWebJobsServiceBus"]);
        endpointConfiguration.SendOnly();
        endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
        endpointConfiguration.Conventions()
            .DefiningCommandsAs(t => Regex.IsMatch(t.Name, "Command(V\\d+)?$"))
            .DefiningEventsAs(t => Regex.IsMatch(t.Name, "Event(V\\d+)?$"));

        var endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
        services.AddSingleton(endpointInstance);
        services.AddSingleton<IMessageSession>(endpointInstance);
        return services;
    }
}
