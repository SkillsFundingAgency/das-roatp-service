using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.RoATPService.Application.Queries.GetAllCourseTypes;
public class GetAllCourseTypesResult
{
    public IEnumerable<CourseTypeSummary> CourseTypes { get; set; } = Enumerable.Empty<CourseTypeSummary>();
}