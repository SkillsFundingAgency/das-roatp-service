using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.RoATPService.Domain.Repositories;
using Entities = SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisationsStatusEvents;

public class GetOrganisationStatusEventsQueryHandler(IOrganisationStatusEventsRepository _repository) : IRequestHandler<GetOrganisationStatusEventsQuery, GetOrganisationStatusEventsQueryResult>
{
    public async Task<GetOrganisationStatusEventsQueryResult> Handle(GetOrganisationStatusEventsQuery request, CancellationToken cancellationToken)
    {
        List<Entities.OrganisationStatusEvent> events = await _repository.GetOrganisationStatusEvents(request.SinceEventId, request.PageSize, request.PageNumber, cancellationToken);
        return new GetOrganisationStatusEventsQueryResult(events.Select(e => new OrganisationStatusEvent(e.Id, e.Ukprn, e.OrganisationStatus, e.CreatedOn)));
    }
}
