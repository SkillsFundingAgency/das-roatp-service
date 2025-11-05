using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.RoATPService.Data.Repositories;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Data.Extensions;

[ExcludeFromCodeCoverage]
public static class AddRoatpDataContextExtension
{
    public static IServiceCollection AddRoatpDataContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<RoatpDataContext>(options =>
            options.UseSqlServer(connectionString, o =>
                o.CommandTimeout((int)TimeSpan.FromMinutes(5).TotalSeconds)));

        services
            .AddHealthChecks()
            .AddDbContextCheck<RoatpDataContext>(name: "sql", tags: ["db", "sql"]);

        RegisterRepositories(services);

        return services;
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddTransient<IOrganisationsRepository, OrganisationsRepository>();
        services.AddTransient<IOrganisationStatusEventsRepository, OrganisationStatusEventsRepository>();
        services.AddTransient<ICourseTypesRepository, CourseTypesRepository>();
        services.AddTransient<IOrganisationCourseTypesRepository, OrganisationCourseTypesRepository>();
        services.AddTransient<IAuditsRepository, AuditsRepository>();
        services.AddTransient<IRemovedReasonsRepository, RemovedReasonsRepository>();
        services.AddTransient<IOrganisationTypesRepository, OrganisationTypesRepository>();
        services.AddTransient<IProviderTypesRepository, ProviderTypesRepository>();
    }
}
