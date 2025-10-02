using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;

public class UpdateOrganisationAllowedShortCoursesCommandHandler(
    IOrganisationsRepository _organisationsRepository,
    IOrganisationCourseTypesRepository _organisationCourseTypesRepository)
    : IRequestHandler<UpdateOrganisationAllowedShortCoursesCommand, ValidatedResponse>
{
    public async Task<ValidatedResponse> Handle(UpdateOrganisationAllowedShortCoursesCommand request, CancellationToken cancellationToken)
    {
        Organisation org = await _organisationsRepository.GetOrganisationByUkprn(request.Ukprn, cancellationToken);

        await _organisationCourseTypesRepository.UpdateOrganisationShortCourseTypes(org.Id, request.CourseTypeIds, cancellationToken);
        return new ValidatedResponse();
    }
}
