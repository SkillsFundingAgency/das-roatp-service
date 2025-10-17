using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisationTypes;

public class GetOrganisationTypesQueryHandler(IOrganisationTypesRepository _organisationTypesRepository, ILogger<GetOrganisationTypesQueryHandler> _logger) : IRequestHandler<GetOrganisationTypesQuery, GetOrganisationTypesQueryResult>
{
    public async Task<GetOrganisationTypesQueryResult> Handle(GetOrganisationTypesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling Get Organisation Types");

        List<OrganisationType> organisationTypes = await _organisationTypesRepository.GetOrganisationTypes(cancellationToken);

        return new() { OrganisationTypes = organisationTypes.Select(o => (OrganisationTypeSummary)o) };
    }
}
