using System.Collections.Generic;
using SFA.DAS.RoATPService.Application.Common;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisations;

public class GetOrganisationsQueryResult
{
    public List<OrganisationModel> Organisations { get; set; } = new();
}