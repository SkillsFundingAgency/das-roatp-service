using System;

namespace SFA.DAS.RoATPService.Domain.Entities;
public class OrganisationStatusEvent
{
    public long Id { get; set; }
    public Common.OrganisationStatus OrganisationStatus { get; set; }
    public DateTime CreatedOn { get; set; }
    public int Ukprn { get; set; }
}
