using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisation;

public class GetOrganisationQueryHandler(
    IOrganisationsRepository _organisationRepository,
    IOrganisationStatusEventsRepository _organisationStatusEventRepository,
    IAuditsRepository _auditsRepository)
    : IRequestHandler<GetOrganisationQuery, GetOrganisationQueryResult>
{
    public async Task<GetOrganisationQueryResult> Handle(GetOrganisationQuery request, CancellationToken cancellationToken)
    {
        Organisation organisation = await _organisationRepository.GetOrganisationByUkprn(request.Ukprn, cancellationToken);
        if (organisation == null)
        {
            return null;
        }

        GetOrganisationQueryResult result = organisation;

        if (organisation.Status == OrganisationStatus.Removed)
        {
            OrganisationStatusEvent latestOrganisationStatusEvent = await _organisationStatusEventRepository.GetLatestStatusChangeEvent(request.Ukprn, organisation.Status, cancellationToken);
            result.RemovedDate = latestOrganisationStatusEvent.CreatedOn;
        }

        var lastUpdatedTime = await _auditsRepository.GetLastUpdatedDateForOrganisation(organisation.Id, cancellationToken);
        result.LastUpdatedDate = lastUpdatedTime ?? organisation.UpdatedAt;

        return result;
    }
}
