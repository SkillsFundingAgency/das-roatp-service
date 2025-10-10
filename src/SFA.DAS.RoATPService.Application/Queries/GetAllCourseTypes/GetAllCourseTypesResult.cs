using SFA.DAS.RoATPService.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.RoATPService.Application.Queries.GetAllCourseTypes;
public class GetAllCourseTypesResult
{
    public IEnumerable<CourseTypeSummary> CourseTypes { get; set; } = Enumerable.Empty<CourseTypeSummary>();
}

public record CourseTypeSummary(int Id, string Name, LearningType LearningType)
{
    public static implicit operator CourseTypeSummary(CourseType source) => new(source.Id, source.Name, source.LearningType);
}