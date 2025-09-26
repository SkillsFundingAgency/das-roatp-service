using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class OrganisationRepository(RoatpDataContext _dataContext) : IOrganisationRepository
{
    public Task<Organisation> GetOrganisationByUkprn(int ukprn, CancellationToken cancellationToken)
        => _dataContext
            .Organisations
            .Include(o => o.OrganisationType)
            .Include(o => o.OrganisationCourseTypes)
            .ThenInclude(oc => oc.CourseType)
            .FirstOrDefaultAsync(o => o.Ukprn == ukprn, cancellationToken);
}
