using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisation;
using SFA.DAS.RoATPService.Domain.Configuration;
using SFA.DAS.RoATPService.Ukrlp.Client;
using SFA.DAS.RoATPService.Ukrlp.Client.Interfaces;

namespace SFA.DAS.RoATPService.Application.Api.AppStart;

[ExcludeFromCodeCoverage]
public static class AddServiceRegistrationsExtensions
{
    public static IServiceCollection AddServiceRegistrations(this IServiceCollection services, IConfiguration configuration)
    {
        RegisterConfigurations(services, configuration);
        services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(GetOrganisationQueryHandler).Assembly));

        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        services.AddHttpClient<IUkrlpApiClient, UkrlpApiClient>();
        services.AddTransient<IUkrlpSoapSerializer, UkrlpSoapSerializer>();

        services.AddValidatorsFromAssembly(typeof(ValidationBehavior<,>).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    private static void RegisterConfigurations(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration.GetSection("RegisterAuditLogSettings").Get<RegisterAuditLogSettings>());

        services.AddSingleton(configuration.GetSection("UkrlpApiAuthentication").Get<UkrlpApiAuthentication>());
    }
}
