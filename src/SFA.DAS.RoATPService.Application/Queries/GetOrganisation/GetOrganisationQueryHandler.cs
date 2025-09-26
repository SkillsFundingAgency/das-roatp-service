using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisation;

public class GetOrganisationQueryHandler(IOrganisationRepository _organisationRepository, IOrganisationStatusEventRepository _organisationStatusEventRepository) : IRequestHandler<GetOrganisationQuery, GetOrganisationQueryResult>
{
    public async Task<GetOrganisationQueryResult> Handle(GetOrganisationQuery request, CancellationToken cancellationToken)
    {
        Organisation organisation = await _organisationRepository.GetOrganisationByUkprn(request.Ukprn, cancellationToken);
        if (organisation == null)
        {
            return null;
        }

        OrganisationStatusEvent latestOrganisationStatusEvent = await _organisationStatusEventRepository.GetLatestStatusChangeEvent(request.Ukprn, organisation.Status, cancellationToken);

        GetOrganisationQueryResult result = organisation;
        result.RemovedDate = latestOrganisationStatusEvent.CreatedOn;

        return result;
    }
}
