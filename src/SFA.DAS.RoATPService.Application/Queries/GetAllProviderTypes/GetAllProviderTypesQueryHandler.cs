using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.Queries.GetAllProviderTypes;

public class GetAllProviderTypesQueryHandler(IProviderTypesRepository _providerTypesRepository) : IRequestHandler<GetAllProviderTypesQuery, GetAllProviderTypesQueryResult>
{
    public async Task<GetAllProviderTypesQueryResult> Handle(GetAllProviderTypesQuery request, CancellationToken cancellationToken)
    {
        List<Domain.Entities.ProviderType> providerTypes = await _providerTypesRepository.GetAll(cancellationToken);

        return new GetAllProviderTypesQueryResult(providerTypes.Select(t => (ProviderTypeModel)t));
    }
}
