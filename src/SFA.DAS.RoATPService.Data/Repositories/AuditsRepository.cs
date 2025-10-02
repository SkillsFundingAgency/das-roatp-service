using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Data.Repositories;

internal class AuditsRepository(RoatpDataContext _dataContext) : IAuditsRepository
{
    public Task<DateTime?> GetLastUpdatedDateForOrganisation(Guid organisationId, CancellationToken cancellationToken)
        =>
        _dataContext.Audits
            .Where(a => a.OrganisationId == organisationId)
            .OrderByDescending(a => a.UpdatedAt)
            .Select(a => a.UpdatedAt)
            .FirstOrDefaultAsync(cancellationToken);
}
