using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Domain.Entities;

public class ProviderType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public virtual ICollection<Organisation> Organisations { get; set; } = [];
}
