using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Application.Queries.GetAllCourseTypes;
public record CourseTypeSummary(int Id, string Name, LearningType LearningType)
{
    public static implicit operator CourseTypeSummary(CourseType source) => new(source.Id, source.Name, source.LearningType);
}