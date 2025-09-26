using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Data.EntityConfigurations;

[ExcludeFromCodeCoverage]
internal class OrganisationStatusEventConfiguration : IEntityTypeConfiguration<OrganisationStatusEvent>
{
    public void Configure(EntityTypeBuilder<OrganisationStatusEvent> builder)
    {
        builder.ToTable("OrganisationStatusEvent");
        builder
            .Property(e => e.Ukprn)
            .HasColumnName("ProviderId")
            .HasColumnType("bigint");
        builder
            .Property(e => e.OrganisationStatus)
            .HasColumnName("OrganisationStatusId")
            .HasConversion<int>();
    }
}
