using MediatR;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;

namespace SFA.DAS.RoATPService.Application.commands.DeleteOrganisationShortCourseTypes;
public record DeleteOrganisationShortCourseTypesCommand(int Ukprn, string RequestingUserId) : IRequest<ValidatedResponse>;