using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        endpointConfiguration.AssemblyScanner().ScanFileSystemAssemblies = false;

        endpointConfiguration.CustomDiagnosticsWriter((diagnostics, _) =>
        {
            Console.WriteLine(diagnostics);
            return Task.CompletedTask;
        });

        var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
        transport.ConnectionString(configuration["NServiceBusConnectionString"]);
        endpointConfiguration.SendOnly();
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();

        endpointConfiguration.Conventions().DefiningEventsAs(t => EventGeneratedRegex().IsMatch(t.Name));

        var decodedLicense = WebUtility.HtmlDecode(configuration["NServiceBusLicense"]);

        endpointConfiguration.License(decodedLicense);

        var endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
        services.AddSingleton(endpointInstance);
        services.AddSingleton<IMessageSession>(endpointInstance);
        return services;
    }

    [GeneratedRegex("Event(V\\d+)?$")]
    private static partial Regex EventGeneratedRegex();
}
