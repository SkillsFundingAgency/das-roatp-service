using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Data.EntityConfigurations;

[ExcludeFromCodeCoverage]
public class OrganisationTypeConfiguration : IEntityTypeConfiguration<OrganisationType>
{
    public void Configure(EntityTypeBuilder<OrganisationType> builder)
    {
        builder.ToTable(nameof(RoatpDataContext.OrganisationTypes));
    }
}
