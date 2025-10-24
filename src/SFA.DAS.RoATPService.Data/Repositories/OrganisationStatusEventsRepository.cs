using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class OrganisationStatusEventsRepository(RoatpDataContext _dataContext) : IOrganisationStatusEventsRepository
{
    public async Task<List<OrganisationStatusEvent>> GetOrganisationStatusHistory(int ukprn, CancellationToken cancellationToken)
        => await _dataContext.OrganisationStatusEvents.Where(e => e.Ukprn == ukprn).ToListAsync(cancellationToken);
}
