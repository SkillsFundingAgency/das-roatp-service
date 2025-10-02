using System;

namespace SFA.DAS.RoATPService.Domain.Entities;

public class Audit
{
    public int Id { get; set; }
    public Guid OrganisationId { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public AuditData AuditData { get; set; }

}
