using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Data.EntityConfigurations;

[ExcludeFromCodeCoverage]
public class OrganisationConfiguration : IEntityTypeConfiguration<Organisation>
{
    public void Configure(EntityTypeBuilder<Organisation> builder)
    {
        builder.ToTable("Organisations");
        builder
            .Property(e => e.Ukprn)
            .HasColumnType("bigint");
        builder
            .Property(e => e.Status)
            .HasConversion<int>();
        builder
            .Property(e => e.ProviderType)
            .HasConversion<int>();
        builder
            .Property(s => s.Status)
            .HasColumnName("StatusId")
            .HasConversion<int>();
        builder
            .Property(e => e.ProviderType)
            .HasColumnName("ProviderTypeId")
            .HasConversion<int>();

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }
}
