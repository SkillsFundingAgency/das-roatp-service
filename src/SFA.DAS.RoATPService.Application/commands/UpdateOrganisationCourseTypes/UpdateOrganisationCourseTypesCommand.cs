using MediatR;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;

public record UpdateOrganisationCourseTypesCommand(int Ukprn, IEnumerable<int> CourseTypeIds, string RequestingUserId) : IRequest<ValidatedResponse>;
