using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class ProviderTypeOrganisationTypesRepository(RoatpDataContext _dataContext) : IProviderTypeOrganisationTypesRepository
{
    public async Task<List<OrganisationType>> GetOrganisationTypeByProviderTypeId(int providerTypeId, CancellationToken cancellationToken)
        => await _dataContext.ProviderTypeOrganisationTypes
            .Where(p => p.ProviderTypeId == providerTypeId)
            .Select(p => p.OrganisationType)
            .ToListAsync(cancellationToken);
}
