using MediatR;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.commands.DeleteOrganisationShortCourseTypes;
public class DeleteOrganisationShortCourseTypesCommandHandler(IOrganisationsRepository _organisationsRepository, IOrganisationCourseTypesRepository _organisationCourseTypesRepository) : IRequestHandler<DeleteOrganisationShortCourseTypesCommand, ValidatedResponse>
{
    public async Task<ValidatedResponse> Handle(DeleteOrganisationShortCourseTypesCommand request, CancellationToken cancellationToken)
    {
        Organisation org = await _organisationsRepository.GetOrganisationByUkprn(request.Ukprn, cancellationToken);

        if (org.OrganisationCourseTypes.Select(o => o.CourseType.LearningType).Contains(LearningType.ShortCourse))
        {
            await _organisationCourseTypesRepository.DeleteOrganisationShortCourseTypes(org.Id, request.RequestingUserId, cancellationToken);
        }

        return new ValidatedResponse();
    }
}