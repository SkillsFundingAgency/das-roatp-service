using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Application.Queries.GetAllProviderTypes;

public record GetAllProviderTypesQueryResult(IEnumerable<ProviderTypeModel> ProviderTypes);

public record ProviderTypeModel(int Id, string Type, string Description)
{
    public static implicit operator ProviderTypeModel(Domain.Entities.ProviderType providerType) =>
        new ProviderTypeModel(providerType.Id, providerType.Name, providerType.Description);
}
