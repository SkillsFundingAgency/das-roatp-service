using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisationStatusHistory;

public record GetOrganisationStatusHistoryQueryResult(IEnumerable<StatusHistoryModel> StatusHistory);
