using System;

namespace SFA.DAS.RoATPService.Domain.Entities;

public class OrganisationAudit
{
    public int Ukprn { get; set; }
    public string LegalName { get; set; }
    public string FieldChanged { get; set; }
    public string PreviousValue { get; set; }
    public string NewValue { get; set; }
    public DateTime? PreviousStatusDate { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
}
