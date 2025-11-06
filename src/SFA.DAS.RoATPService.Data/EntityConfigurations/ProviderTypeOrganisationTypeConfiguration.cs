using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Data.EntityConfigurations;

[ExcludeFromCodeCoverage]
public class ProviderTypeOrganisationTypeConfiguration : IEntityTypeConfiguration<ProviderTypeOrganisationType>
{
    public void Configure(EntityTypeBuilder<ProviderTypeOrganisationType> builder)
    {
        builder.ToTable("ProviderTypeOrganisationTypes");
        builder.HasOne(p => p.OrganisationType)
               .WithMany()
               .HasForeignKey(p => p.OrganisationTypeId);
    }
}
