using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Data.Repositories;

internal class OrganisationStatusEventsRepository(RoatpDataContext _dataContext) : IOrganisationStatusEventsRepository
{
    public async Task<List<OrganisationStatusEvent>> GetOrganisationStatusEvents(int sinceEventId, int pageSize, int pageNumber, CancellationToken cancellationToken)
        => await _dataContext.OrganisationStatusEvents
            .Where(e => e.Id > sinceEventId)
            .OrderBy(e => e.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public async Task<List<OrganisationStatusEvent>> GetOrganisationStatusEventsByUkprn(int ukprn, CancellationToken cancellationToken)
        => await _dataContext.OrganisationStatusEvents.Where(e => e.Ukprn == ukprn).ToListAsync(cancellationToken);
}
