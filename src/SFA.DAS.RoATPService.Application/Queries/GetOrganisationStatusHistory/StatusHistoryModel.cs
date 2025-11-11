using System;
using SFA.DAS.RoATPService.Domain.Common;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisationStatusHistory;

public record StatusHistoryModel(OrganisationStatus Status, DateTime AppliedDate);
