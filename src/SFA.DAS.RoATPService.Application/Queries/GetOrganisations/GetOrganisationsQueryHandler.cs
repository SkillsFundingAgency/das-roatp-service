using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.RoATPService.Application.Common;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisations;
public class GetOrganisationsQueryHandler(
        IOrganisationsRepository _organisationRepository)
    : IRequestHandler<GetOrganisationsQuery, GetOrganisationsQueryResult>
{
    public async Task<GetOrganisationsQueryResult> Handle(GetOrganisationsQuery request,
        CancellationToken cancellationToken)
    {
        var organisations = await _organisationRepository.GetOrganisations(cancellationToken);

        var organisationsReturned = new GetOrganisationsQueryResult
        { Organisations = organisations.Select(o => (OrganisationModel)o).ToList() };

        return organisationsReturned;
    }
}