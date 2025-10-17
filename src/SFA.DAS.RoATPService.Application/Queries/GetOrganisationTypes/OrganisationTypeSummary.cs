using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisationTypes;

public record OrganisationTypeSummary(int Id, string Description)
{
    public static implicit operator OrganisationTypeSummary(OrganisationType source) => new(source.Id, source.Type);
}
