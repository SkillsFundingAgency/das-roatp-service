using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Data.EntityConfigurations;

public class ProviderTypeConfiguration : IEntityTypeConfiguration<ProviderType>
{
    public void Configure(EntityTypeBuilder<ProviderType> builder)
    {
        builder.ToTable("ProviderTypes");
        builder.Property(pt => pt.Name).HasColumnName("ProviderType");
    }
}
