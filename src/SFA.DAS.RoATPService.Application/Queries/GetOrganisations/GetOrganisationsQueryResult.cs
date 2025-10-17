using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisations;

public class GetOrganisationsQueryResult
{
    public List<GetOrganisationDetails> Organisations { get; set; } = new();
}