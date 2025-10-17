using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Data.Repositories;

[ExcludeFromCodeCoverage]
public class OrganisationTypesRepository(RoatpDataContext _dataContext) : IOrganisationTypesRepository
{
    public Task<List<Domain.Entities.OrganisationType>> GetOrganisationTypes(CancellationToken cancellationToken) => _dataContext.OrganisationTypes.ToListAsync(cancellationToken);
}