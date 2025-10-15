using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.RoATPService.Domain.Entities;

public class RemovedReason
{
    public int Id { get; set; }
    [Column("RemovedReason")]
    public string Reason { get; set; }
    public string Description { get; set; }
}
