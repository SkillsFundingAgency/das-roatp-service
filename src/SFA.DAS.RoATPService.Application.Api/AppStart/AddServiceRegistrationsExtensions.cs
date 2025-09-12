using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.RoATPService.Api.Client;
using SFA.DAS.RoATPService.Api.Client.AutoMapper;
using SFA.DAS.RoATPService.Api.Client.Interfaces;
using SFA.DAS.RoATPService.Application.Api.Helpers;
using SFA.DAS.RoATPService.Application.Handlers;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Mappers;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Application.Validators;
using SFA.DAS.RoATPService.Data;
using SFA.DAS.RoATPService.Data.Helpers;
using SFA.DAS.RoATPService.Infrastructure.Database;
using SFA.DAS.RoATPService.Infrastructure.Interfaces;
using SFA.DAS.RoATPService.Settings;

namespace SFA.DAS.RoATPService.Application.Api.AppStart;

public static class AddServiceRegistrationsExtensions
{
    public static IServiceCollection AddServiceRegistrations(this IServiceCollection services, IConfiguration configuration)
    {
        AddMappings();
        RegisterConfigurations(services, configuration);
        AddHealthChecks(services, configuration);
        services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(GetProviderTypesHandler).Assembly));

        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<IDownloadRegisterRepository, DownloadRegisterRepository>();
        services.AddTransient<IFatDataExportRepository, FatDataExportRepository>();
        services.AddTransient<ILookupDataRepository, LookupDataRepository>();
        services.AddTransient<IOrganisationRepository, OrganisationRepository>();
        services.AddTransient<IDuplicateCheckRepository, DuplicateCheckRepository>();
        services.AddTransient<ICreateOrganisationRepository, CreateOrganisationRepository>();
        services.AddTransient<IOrganisationSearchRepository, OrganisationSearchRepository>();
        services.AddTransient<IUpdateOrganisationRepository, UpdateOrganisationRepository>();
        services.AddTransient<IOrganisationCategoryValidator, OrganisationCategoryValidator>();
        services.AddTransient<IDataTableHelper, DataTableHelper>();
        services.AddTransient<ICacheHelper, CacheHelper>();
        services.AddTransient<IProviderTypeValidator, ProviderTypeValidator>();
        services.AddTransient<IOrganisationSearchValidator, OrganisationSearchValidator>();
        services.AddTransient<IOrganisationValidator, OrganisationValidator>();
        services.AddTransient<IOrganisationSearchValidator, OrganisationSearchValidator>();
        services.AddTransient<IMapCreateOrganisationRequestToCommand, MapCreateOrganisationRequestToCommand>();
        services.AddTransient<ITextSanitiser, TextSanitiser>();
        services.AddHttpClient<IUkrlpApiClient, UkrlpApiClient>();
        services.AddTransient<IAuditLogService, AuditLogService>();
        services.AddTransient<IOrganisationStatusManager, OrganisationStatusManager>();
        services.AddTransient<IUkrlpSoapSerializer, UkrlpSoapSerializer>();
        services.AddTransient<IEventsRepository, EventsRepository>();

        services.AddTransient<IDbConnectionHelper, DbConnectionHelper>();
        services.AddTransient<IFatDataExportService, FatDataExportService>();

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
        var auditLogSettings = new RegisterAuditLogSettings();
        configuration.Bind("RegisterAuditLogSettings", auditLogSettings);
        services.AddSingleton(auditLogSettings);

        WebConfiguration webConfiguration = new()
        {
            SqlConnectionString = configuration["SqlConnectionString"],
            SessionRedisConnectionString = configuration["SessionRedisConnectionString"],
            UkrlpApiAuthentication = configuration.GetSection("UkrlpApiAuthentication").Get<UkrlpApiAuthentication>()
        };

        services.AddSingleton(webConfiguration);
    }

    private static void AddHealthChecks(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddSqlServer(configuration["SqlConnectionString"], name: "sql", tags: ["db", "sql"]);
    }
}
