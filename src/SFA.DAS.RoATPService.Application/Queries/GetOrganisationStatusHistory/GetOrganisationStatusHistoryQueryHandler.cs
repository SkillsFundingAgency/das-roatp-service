using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisationStatusHistory;

public class GetOrganisationStatusHistoryQueryHandler(IOrganisationStatusEventsRepository _repository) : IRequestHandler<GetOrganisationStatusHistoryQuery, GetOrganisationStatusHistoryQueryResult>
{
    public async Task<GetOrganisationStatusHistoryQueryResult> Handle(GetOrganisationStatusHistoryQuery request, CancellationToken cancellationToken)
    {
        List<OrganisationStatusEvent> statusEvents = await _repository.GetOrganisationStatusHistory(request.Ukprn, cancellationToken);
        var statusHistory = statusEvents.Select(e => new StatusHistoryModel(e.OrganisationStatus, e.CreatedOn));
        return new GetOrganisationStatusHistoryQueryResult(statusHistory);
    }
}
