using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Application.Common.Models;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.commands.DeleteOrganisationShortCourseTypes;
public class DeleteOrganisationShortCourseTypesCommandHandler(IOrganisationsRepository _organisationsRepository, IOrganisationCourseTypesRepository _organisationCourseTypesRepository, ILogger<DeleteOrganisationShortCourseTypesCommandHandler> _logger) : IRequestHandler<DeleteOrganisationShortCourseTypesCommand, ValidatedResponse<SuccessModel>>
{
    public async Task<ValidatedResponse<SuccessModel>> Handle(DeleteOrganisationShortCourseTypesCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handle DeleteShortCourseTypes request for ukprn {ukprn}.", request.Ukprn);

        Organisation organisation = await _organisationsRepository.GetOrganisationByUkprn(request.Ukprn, cancellationToken);

        if (organisation is null) return new ValidatedResponse<SuccessModel>(new SuccessModel(false));

        if (organisation.OrganisationCourseTypes.Select(o => o.CourseType.LearningType).Contains(LearningType.ShortCourse))
        {
            await _organisationCourseTypesRepository.DeleteOrganisationShortCourseTypes(organisation.Id, request.RequestingUserId, cancellationToken);
        }

        return new ValidatedResponse<SuccessModel>(new SuccessModel(true));
    }
}