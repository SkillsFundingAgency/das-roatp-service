using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;

namespace SFA.DAS.RoATPService.Application.Api.AppStart;

[ExcludeFromCodeCoverage]
public static partial class AddNServiceBusExtension
{
    public static IServiceCollection AddNServiceBus(this IServiceCollection services, IConfiguration configuration)
    {
        var endpointConfiguration = new EndpointConfiguration("SFA.DAS.RoATPService");

        var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
        transport.ConnectionString(configuration["AzureWebJobsServiceBus"]);
        endpointConfiguration.SendOnly();
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        endpointConfiguration.Conventions()
            .DefiningEventsAs(t => EventGeneratedRegex().IsMatch(t.Name));

        var endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
        services.AddSingleton(endpointInstance);
        services.AddSingleton<IMessageSession>(endpointInstance);
        return services;
    }

    [GeneratedRegex("Event(V\\d+)?$")]
    private static partial Regex EventGeneratedRegex();
}
