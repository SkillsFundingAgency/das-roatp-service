using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using Organisation = SFA.DAS.RoATPService.Domain.Entities.Organisation;

namespace SFA.DAS.RoATPService.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class OrganisationsRepository(RoatpDataContext _dataContext) : IOrganisationsRepository
{
    public Task<Organisation> GetOrganisationByUkprn(int ukprn, CancellationToken cancellationToken)
        => _dataContext
            .Organisations
            .Include(o => o.RemovedReason)
            .Include(o => o.OrganisationType)
            .Include(o => o.OrganisationCourseTypes)
            .ThenInclude(oc => oc.CourseType)
            .FirstOrDefaultAsync(o => o.Ukprn == ukprn, cancellationToken);

    public Task<List<Organisation>> GetOrganisations(CancellationToken cancellationToken)
        => _dataContext
            .Organisations
            .Include(o => o.RemovedReason)
            .Include(o => o.OrganisationType)
            .Include(o => o.OrganisationCourseTypes)
            .ThenInclude(oc => oc.CourseType)
            .ToListAsync(cancellationToken);

    public Task<List<Organisation>> GetOrganisationsBySearchTerm(string searchTerm, CancellationToken cancellationToken)
        => _dataContext
            .Organisations
            .Include(o => o.RemovedReason)
            .Include(o => o.OrganisationType)
            .Include(o => o.OrganisationCourseTypes)
            .ThenInclude(oc => oc.CourseType)
            .Where(o => o.LegalName.Contains(searchTerm) || o.TradingName.Contains(searchTerm))
            .ToListAsync(cancellationToken);

    public async Task UpdateOrganisation(Organisation organisation, Audit audit, OrganisationStatusEvent statusEvent,
        CancellationToken cancellationToken)
    {
        if (statusEvent != null)
        {
            _dataContext.OrganisationStatusEvents.Add(statusEvent);
        }

        _dataContext.Organisations.Update(organisation);
        _dataContext.Audits.Add(audit);

        await _dataContext.SaveChangesAsync(cancellationToken);
    }
}
