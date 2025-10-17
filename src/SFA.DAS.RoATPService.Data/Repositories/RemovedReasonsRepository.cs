using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Domain.Repositories;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class RemovedReasonsRepository(RoatpDataContext _dataContext) : IRemovedReasonsRepository
{
    public Task<List<Domain.Entities.RemovedReason>> GetAllRemovedReasons(CancellationToken cancellationToken) => _dataContext.RemovedReasons.ToListAsync(cancellationToken);
}