using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.RoATPService.Application.Common.Models;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;

public class UpdateOrganisationCourseTypesCommandHandler(
    IOrganisationsRepository _organisationsRepository,
    IOrganisationCourseTypesRepository _organisationCourseTypesRepository)
    : IRequestHandler<UpdateOrganisationCourseTypesCommand, ValidatedResponse<SuccessModel>>
{
    public async Task<ValidatedResponse<SuccessModel>> Handle(UpdateOrganisationCourseTypesCommand request, CancellationToken cancellationToken)
    {
        Organisation org = await _organisationsRepository.GetOrganisationByUkprn(request.Ukprn, cancellationToken);
        if (org == null) return new ValidatedResponse<SuccessModel>(new SuccessModel(false));

        await _organisationCourseTypesRepository.UpdateOrganisationCourseTypes(org, request.CourseTypeIds, request.RequestingUserId, cancellationToken);
        return new ValidatedResponse<SuccessModel>(new SuccessModel(true));
    }
}
