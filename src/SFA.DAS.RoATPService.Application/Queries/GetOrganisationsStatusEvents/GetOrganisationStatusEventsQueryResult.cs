using System;
using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisationsStatusEvents;

public record GetOrganisationStatusEventsQueryResult(IEnumerable<OrganisationStatusEvent> Events);

public record OrganisationStatusEvent(long Id, int ProviderId, string Event, DateTime CreatedOn);
