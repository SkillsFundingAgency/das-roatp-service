using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisationTypes;
public class GetOrganisationTypesQueryResult
{
    public IEnumerable<OrganisationTypeSummary> OrganisationTypes { get; set; } = Enumerable.Empty<OrganisationTypeSummary>();
}
