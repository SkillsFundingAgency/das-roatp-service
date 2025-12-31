using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Domain.Entities;
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

    public Task<List<OrganisationAudit>> GetOrganisationAuditRecords(CancellationToken cancellationToken)
        => _dataContext.OrganisationAudits.FromSql($"EXECUTE dbo.RoATP_Audit_History").ToListAsync(cancellationToken);
}
