using System.Collections.Generic;
using MediatR;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;

namespace SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;

public record UpdateOrganisationAllowedShortCoursesCommand(int Ukprn, IEnumerable<int> CourseTypeIds, string RequestingUserId) : IRequest<ValidatedResponse>;
