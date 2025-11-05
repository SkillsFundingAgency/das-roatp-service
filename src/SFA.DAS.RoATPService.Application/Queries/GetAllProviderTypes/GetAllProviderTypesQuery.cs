using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.Queries.GetAllProviderTypes;
public record GetAllProviderTypesQuery : IRequest<GetAllProviderTypesQueryResult>;

public record GetAllProviderTypesQueryResult(IEnumerable<ProviderTypeModel> ProviderTypes);

public record ProviderTypeModel(int Id, string Name, string Description)
{
    public static implicit operator ProviderTypeModel(Domain.Entities.ProviderType providerType) =>
        new ProviderTypeModel(providerType.Id, providerType.Name, providerType.Description);
}

public class GetAllProviderTypesQueryHandler(IProviderTypesRepository _providerTypesRepository) : IRequestHandler<GetAllProviderTypesQuery, GetAllProviderTypesQueryResult>
{
    public async Task<GetAllProviderTypesQueryResult> Handle(GetAllProviderTypesQuery request, CancellationToken cancellationToken)
    {
        List<Domain.Entities.ProviderType> providerTypes = await _providerTypesRepository.GetAll(cancellationToken);

        return new GetAllProviderTypesQueryResult(providerTypes.Select(t => (ProviderTypeModel)t));
    }
}
