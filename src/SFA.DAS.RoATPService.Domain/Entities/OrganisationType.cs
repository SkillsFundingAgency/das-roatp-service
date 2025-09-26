using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Domain.Entities;

public class OrganisationType
{
    public int Id { get; set; }
    public string Type { get; set; }

    public virtual ICollection<Organisation> Organisations { get; set; } = [];
}

