using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using SFA.DAS.RoATPService.Domain;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Data.EntityConfigurations;

public class AuditConfiguration : IEntityTypeConfiguration<Audit>
{
    public void Configure(EntityTypeBuilder<Audit> builder)
    {
        builder.ToTable(nameof(Audit));

        builder.Property(a => a.AuditData)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<AuditData>(v));
    }
}
