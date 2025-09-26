using System;
using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Domain.Entities;

public class Organisation
{
    public Guid Id { get; set; }
    public OrganisationStatus Status { get; set; }
    public ProviderType ProviderType { get; set; }
    public int OrganisationTypeId { get; set; }
    public int Ukprn { get; set; }
    public string LegalName { get; set; }
    public string TradingName { get; set; }
    public DateTime StatusDate { get; set; }
    public OrganisationData OrganisationData { get; set; }

    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual OrganisationType OrganisationType { get; set; }
    public virtual ICollection<OrganisationCourseType> OrganisationCourseTypes { get; set; } = [];
}
