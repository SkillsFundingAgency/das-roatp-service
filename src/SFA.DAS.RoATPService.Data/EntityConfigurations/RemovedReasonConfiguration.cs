using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Data.EntityConfigurations;

[ExcludeFromCodeCoverage]
public class RemovedReasonConfiguration : IEntityTypeConfiguration<RemovedReason>
{
    public void Configure(EntityTypeBuilder<RemovedReason> builder)
    {
        builder.ToTable(nameof(RoatpDataContext.RemovedReasons));
    }
}
