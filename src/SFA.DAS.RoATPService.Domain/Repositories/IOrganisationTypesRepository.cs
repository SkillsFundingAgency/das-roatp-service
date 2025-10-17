using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Domain.Repositories;

public interface IOrganisationTypesRepository
{
    Task<List<Entities.OrganisationType>> GetOrganisationTypes(CancellationToken cancellationToken);
}