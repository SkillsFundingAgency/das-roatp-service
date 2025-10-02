using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Domain.Repositories;
public interface IOrganisationCourseTypesRepository
{
    Task UpdateOrganisationShortCourseTypes(Guid organisationId, IEnumerable<int> courseTypeIds, string userId, CancellationToken cancellationToken);
}
