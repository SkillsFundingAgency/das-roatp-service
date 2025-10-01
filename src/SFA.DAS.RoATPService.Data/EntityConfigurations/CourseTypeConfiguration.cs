using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Data.EntityConfigurations;

[ExcludeFromCodeCoverage]
public class CourseTypeConfiguration : IEntityTypeConfiguration<CourseType>
{
    public void Configure(EntityTypeBuilder<CourseType> builder)
    {
        builder.ToTable(nameof(RoatpDataContext.CourseTypes));

        builder.Property(ct => ct.LearningType).HasConversion<string>();
    }
}
