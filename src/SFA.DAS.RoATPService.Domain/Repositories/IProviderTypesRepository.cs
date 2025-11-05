using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Domain.Repositories;

public interface IProviderTypesRepository
{
    Task<List<Entities.ProviderType>> GetAll(CancellationToken cancellationToken);
}
