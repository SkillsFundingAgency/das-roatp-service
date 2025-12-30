using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.RoATPService.Api.Client;
using SFA.DAS.RoATPService.Api.Client.AutoMapper;
using SFA.DAS.RoATPService.Api.Client.Interfaces;
using SFA.DAS.RoATPService.Application.Api.Configuration;
using SFA.DAS.RoATPService.Application.Handlers;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Application.Validators;
using SFA.DAS.RoATPService.Data;
using SFA.DAS.RoATPService.Data.Helpers;
using SFA.DAS.RoATPService.Domain.Configuration;
using SFA.DAS.RoATPService.Infrastructure.Database;
using SFA.DAS.RoATPService.Infrastructure.Interfaces;

namespace SFA.DAS.RoATPService.Application.Api.AppStart;

[ExcludeFromCodeCoverage]
public static class AddServiceRegistrationsExtensions
{
    public static IServiceCollection AddServiceRegistrations(this IServiceCollection services, IConfiguration configuration)
    {
        AddMappings();
        RegisterConfigurations(services, configuration);
        services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(GetProviderTypesHandler).Assembly));

        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<IDownloadRegisterRepository, DownloadRegisterRepository>();
        services.AddTransient<ILookupDataRepository, LookupDataRepository>();
        services.AddTransient<IOrganisationSearchRepository, OrganisationSearchRepository>();
        services.AddTransient<IOrganisationCategoryValidator, OrganisationCategoryValidator>();
        services.AddTransient<ICacheHelper, CacheHelper>();
        services.AddTransient<IProviderTypeValidator, ProviderTypeValidator>();
        services.AddTransient<IOrganisationSearchValidator, OrganisationSearchValidator>();
        services.AddTransient<IOrganisationSearchValidator, OrganisationSearchValidator>();
        services.AddTransient<ITextSanitiser, TextSanitiser>();
        services.AddHttpClient<IUkrlpApiClient, UkrlpApiClient>();
        services.AddTransient<IOrganisationStatusManager, OrganisationStatusManager>();
        services.AddTransient<IUkrlpSoapSerializer, UkrlpSoapSerializer>();
        services.AddTransient<IEventsRepository, EventsRepository>();

        services.AddTransient<IDbConnectionHelper, DbConnectionHelper>();

        services.AddValidatorsFromAssembly(typeof(ValidationBehavior<,>).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    public static void AddMappings()
    {
        Mapper.Reset();

        Mapper.Initialize(cfg =>
        {
            cfg.AddProfile<UkrlpVerificationDetailsProfile>();
            cfg.AddProfile<UkrlpContactPersonalDetailsProfile>();
            cfg.AddProfile<UkrlpContactAddressProfile>();
            cfg.AddProfile<UkrlpProviderAliasProfile>();
            cfg.AddProfile<UkrlpProviderContactProfile>();
            cfg.AddProfile<UkrlpProviderDetailsProfile>();
        });

        Mapper.AssertConfigurationIsValid();
    }

    private static void RegisterConfigurations(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration.GetSection("RegisterAuditLogSettings").Get<RegisterAuditLogSettings>());

        services.AddSingleton(configuration.GetSection("UkrlpApiAuthentication").Get<UkrlpApiAuthentication>());
    }
}
