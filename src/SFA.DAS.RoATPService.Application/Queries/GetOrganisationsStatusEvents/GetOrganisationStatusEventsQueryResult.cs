using System;
using System.Collections.Generic;
using SFA.DAS.RoATPService.Domain.Common;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisationsStatusEvents;

public record GetOrganisationStatusEventsQueryResult(IEnumerable<OrganisationStatusEvent> Events);

public record OrganisationStatusEvent(long EventId, int Ukprn, OrganisationStatus Status, DateTime CreatedOn);
