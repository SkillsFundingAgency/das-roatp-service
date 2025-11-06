using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisationTypes;

public class GetOrganisationTypesQueryHandler(IOrganisationTypesRepository _organisationTypesRepository, IProviderTypeOrganisationTypesRepository _providerTypeOrganisationTypesRepository, ILogger<GetOrganisationTypesQueryHandler> _logger) : IRequestHandler<GetOrganisationTypesQuery, ValidatedResponse<GetOrganisationTypesQueryResult>>
{
    public async Task<ValidatedResponse<GetOrganisationTypesQueryResult>> Handle(GetOrganisationTypesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling Get Organisation Types");
        var task = request.ProviderTypeId == null ? _organisationTypesRepository.GetOrganisationTypes(cancellationToken) : _providerTypeOrganisationTypesRepository.GetOrganisationTypeByProviderTypeId(request.ProviderTypeId.Value, cancellationToken);

        List<OrganisationType> organisationTypes = await task;
        var result = new GetOrganisationTypesQueryResult() { OrganisationTypes = organisationTypes.Select(o => (OrganisationTypeSummary)o) };
        return new ValidatedResponse<GetOrganisationTypesQueryResult>(result);
    }
}
