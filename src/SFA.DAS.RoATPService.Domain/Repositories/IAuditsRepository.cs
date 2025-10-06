using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Domain.Repositories;

public interface IAuditsRepository
{
    Task<DateTime?> GetLastUpdatedDateForOrganisation(Guid organisationId, CancellationToken cancellationToken);
}
