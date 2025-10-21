using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisations;

public class GetOrganisationsQueryResult
{
    public List<OrganisationModel> Organisations { get; set; } = new();
}