using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Data.EntityConfigurations;

[ExcludeFromCodeCoverage]
public class OrganisationCourseTypeConfiguration : IEntityTypeConfiguration<OrganisationCourseType>
{
    public void Configure(EntityTypeBuilder<OrganisationCourseType> builder)
    {
        builder.ToTable(nameof(RoatpDataContext.OrganisationCourseTypes));

        builder
            .HasOne(oct => oct.Organisation)
            .WithMany(o => o.OrganisationCourseTypes)
            .HasPrincipalKey(o => o.Id)
            .HasForeignKey(c => c.OrganisationId);

        builder
            .HasOne(oct => oct.CourseType)
            .WithMany(o => o.OrganisationCourseTypes)
            .HasPrincipalKey(o => o.Id)
            .HasForeignKey(c => c.CourseTypeId);
    }
}
