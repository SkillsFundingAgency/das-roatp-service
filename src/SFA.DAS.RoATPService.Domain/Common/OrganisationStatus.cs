using System.ComponentModel;

namespace SFA.DAS.RoATPService.Domain.Common;

public enum OrganisationStatus
{
    [Description("REMOVED")]
    Removed = 0,
    [Description("ACTIVE")]
    Active = 1,
    [Description("ACTIVENOSTARTS")]
    ActiveNoStarts = 2,
    [Description("INITIATED")]
    OnBoarding = 3
}

