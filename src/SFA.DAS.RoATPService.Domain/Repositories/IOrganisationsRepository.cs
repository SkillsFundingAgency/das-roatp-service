using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Domain.Repositories;

public interface IOrganisationsRepository
{
    Task<Entities.Organisation> GetOrganisationByUkprn(int ukprn, CancellationToken cancellationToken);
    Task<List<Entities.Organisation>> GetOrganisations(CancellationToken cancellationToken);
    Task UpdateOrganisation(Entities.Organisation organisation, Audit audit, OrganisationStatusEvent statusEvent, bool removeShortCourses, string userId, CancellationToken cancellationToken);
}
