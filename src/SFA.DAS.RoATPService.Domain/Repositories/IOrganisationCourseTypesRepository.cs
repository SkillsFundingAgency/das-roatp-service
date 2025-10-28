using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Domain.Repositories;
public interface IOrganisationCourseTypesRepository
{
    Task UpdateOrganisationCourseTypes(Domain.Entities.Organisation organisation, IEnumerable<int> courseTypeIds, string userId, CancellationToken cancellationToken);
    Task DeleteOrganisationShortCourseTypes(Domain.Entities.Organisation organisation, string userId, CancellationToken cancellationToken);
}
