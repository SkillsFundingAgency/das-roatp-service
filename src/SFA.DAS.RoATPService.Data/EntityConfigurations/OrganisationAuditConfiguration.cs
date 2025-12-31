using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Data.EntityConfigurations;

[ExcludeFromCodeCoverage]
public class OrganisationAuditConfiguration : IEntityTypeConfiguration<OrganisationAudit>
{
    public void Configure(EntityTypeBuilder<OrganisationAudit> builder)
    {
        builder
            .HasNoKey()
            .Property(o => o.Ukprn).HasColumnType("bigint");
    }
}
