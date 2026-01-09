using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Domain.Repositories;

public interface IOrganisationsRepository
{
    Task<Organisation> GetOrganisationByUkprn(int ukprn, CancellationToken cancellationToken);
    Task<List<Organisation>> GetOrganisations(CancellationToken cancellationToken);
    Task<List<Organisation>> GetOrganisationsBySearchTerm(string searchTerm, CancellationToken cancellationToken);
    Task UpdateOrganisation(Organisation organisation, Audit audit, OrganisationStatusEvent statusEvent, CancellationToken cancellationToken);
    Task CreateOrganisation(Organisation organisation, Audit audit, OrganisationStatusEvent statusEvent, CancellationToken cancellationToken);
}
