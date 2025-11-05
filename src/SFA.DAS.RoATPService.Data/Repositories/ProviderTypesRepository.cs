using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class ProviderTypesRepository(RoatpDataContext _dataContext) : IProviderTypesRepository
{
    public async Task<List<ProviderType>> GetAll(CancellationToken cancellationToken)
    {
        return await _dataContext.ProviderTypes.ToListAsync(cancellationToken);
    }
}
