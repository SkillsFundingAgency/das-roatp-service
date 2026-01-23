using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Domain.Repositories;

public interface IOrganisationStatusEventsRepository
{
    Task<List<OrganisationStatusEvent>> GetOrganisationStatusEventsByUkprn(int ukprn, CancellationToken cancellationToken);

    Task<List<OrganisationStatusEvent>> GetOrganisationStatusEvents(int sinceEventId, int pageSize, int pageNumber, CancellationToken cancellationToken);
}
