using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.RoATPService.Application.Common;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisation;

public class GetOrganisationQueryHandler(IOrganisationsRepository _organisationRepository)
    : IRequestHandler<GetOrganisationQuery, OrganisationModel>
{
    public async Task<OrganisationModel> Handle(GetOrganisationQuery request, CancellationToken cancellationToken)
    {
        Organisation organisation = await _organisationRepository.GetOrganisationByUkprn(request.Ukprn, cancellationToken);
        if (organisation == null)
        {
            return null;
        }

        OrganisationModel result = organisation;

        return result;
    }
}
