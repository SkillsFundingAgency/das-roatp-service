using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Data.EntityConfigurations;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Data;
public class RoatpDataContext : DbContext
{
    public DbSet<Organisation> Organisations => Set<Organisation>();
    public DbSet<OrganisationType> OrganisationTypes => Set<OrganisationType>();
    public DbSet<OrganisationStatusEvent> OrganisationStatusEvents => Set<OrganisationStatusEvent>();
    public DbSet<OrganisationCourseType> OrganisationCourseTypes => Set<OrganisationCourseType>();
    public DbSet<CourseType> CourseTypes => Set<CourseType>();


    public RoatpDataContext(DbContextOptions<RoatpDataContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrganisationConfiguration).Assembly);
    }
}

