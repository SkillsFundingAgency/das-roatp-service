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
    public Task<OrganisationStatusEvent> GetLatestStatusChangeEvent(int ukprn, OrganisationStatus status, CancellationToken cancellationToken)
        => _dataContext
            .OrganisationStatusEvents
            .Where(e => e.Ukprn == ukprn && e.OrganisationStatus == status)
            .OrderByDescending(e => e.CreatedOn)
            .FirstOrDefaultAsync(cancellationToken);
}
