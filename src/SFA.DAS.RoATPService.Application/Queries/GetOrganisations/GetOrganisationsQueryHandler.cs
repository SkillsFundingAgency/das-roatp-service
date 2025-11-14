using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.RoATPService.Application.Common;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisations;

public class GetOrganisationsQueryHandler(
        IOrganisationsRepository _organisationRepository)
    : IRequestHandler<GetOrganisationsQuery, GetOrganisationsQueryResult>
{
    public async Task<GetOrganisationsQueryResult> Handle(GetOrganisationsQuery request,
        CancellationToken cancellationToken)
    {
        List<Organisation> organisations = await
            (string.IsNullOrWhiteSpace(request.SearchTerm)
            ? _organisationRepository.GetOrganisations(cancellationToken)
            : _organisationRepository.GetOrganisationsBySearchTerm(request.SearchTerm, cancellationToken));

        var organisationsReturned = new GetOrganisationsQueryResult
        {
            Organisations = organisations.Select(o => (OrganisationModel)o).ToList()
        };

        return organisationsReturned;
    }
}
