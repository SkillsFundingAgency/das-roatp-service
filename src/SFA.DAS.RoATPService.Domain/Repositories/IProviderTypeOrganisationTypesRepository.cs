using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Domain.Repositories;

public interface IProviderTypeOrganisationTypesRepository
{
    Task<List<Entities.OrganisationType>> GetOrganisationTypeByProviderTypeId(int providerTypeId, CancellationToken cancellationToken);
}
