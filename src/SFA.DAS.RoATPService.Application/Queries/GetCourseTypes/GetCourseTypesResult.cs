using SFA.DAS.RoATPService.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.RoATPService.Application.Queries.GetCourseTypes;
public class GetCourseTypesResult
{
    public IEnumerable<CourseTypes> CourseTypes { get; set; } = Enumerable.Empty<CourseTypes>();
}

public record CourseTypes(int Id, string Name, LearningType LearningType)
{
    public static implicit operator CourseTypes(CourseType source) => new(source.Id, source.Name, source.LearningType);
}