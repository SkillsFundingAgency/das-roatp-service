namespace SFA.DAS.RoATPService.Domain.Entities;

public class ProviderTypeOrganisationType
{
    public int Id { get; set; }
    public int ProviderTypeId { get; set; }
    public int OrganisationTypeId { get; set; }

    public virtual OrganisationType OrganisationType { get; set; }
}
