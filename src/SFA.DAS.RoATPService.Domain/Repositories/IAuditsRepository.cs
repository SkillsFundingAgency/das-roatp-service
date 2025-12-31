using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Domain.Repositories;

public interface IAuditsRepository
{
    Task<DateTime?> GetLastUpdatedDateForOrganisation(Guid organisationId, CancellationToken cancellationToken);

    Task<List<OrganisationAudit>> GetOrganisationAuditRecords(CancellationToken cancellationToken);
}
