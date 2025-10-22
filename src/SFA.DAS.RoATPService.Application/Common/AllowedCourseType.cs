using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Application.Common;

public record AllowedCourseType(int CourseTypeId, string CourseTypeName, LearningType LearningType);