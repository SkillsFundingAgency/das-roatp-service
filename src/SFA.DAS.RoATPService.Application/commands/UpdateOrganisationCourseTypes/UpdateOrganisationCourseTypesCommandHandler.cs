using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;

public class UpdateOrganisationCourseTypesCommandHandler(
    IOrganisationsRepository _organisationsRepository,
    IOrganisationCourseTypesRepository _organisationCourseTypesRepository)
    : IRequestHandler<UpdateOrganisationCourseTypesCommand, ValidatedResponse>
{
    public async Task<ValidatedResponse> Handle(UpdateOrganisationCourseTypesCommand request, CancellationToken cancellationToken)
    {
        Organisation org = await _organisationsRepository.GetOrganisationByUkprn(request.Ukprn, cancellationToken);

        await _organisationCourseTypesRepository.UpdateOrganisationCourseTypes(org, request.CourseTypeIds, request.RequestingUserId, cancellationToken);
        return new ValidatedResponse();
    }
}
