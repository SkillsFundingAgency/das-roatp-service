using System;

namespace SFA.DAS.RoATPService.Domain.Entities;
public class OrganisationData
{
    public string CompanyNumber { get; set; }
    public string CharityNumber { get; set; }
    public RemovedReason RemovedReason { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? ApplicationDeterminedDate { get; set; }
}
