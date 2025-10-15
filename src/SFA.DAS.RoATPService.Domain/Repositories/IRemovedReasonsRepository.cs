using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Domain.Repositories;
public interface IRemovedReasonsRepository
{
    Task<List<Entities.RemovedReason>> GetAllRemovedReasons(CancellationToken cancellationToken);
}