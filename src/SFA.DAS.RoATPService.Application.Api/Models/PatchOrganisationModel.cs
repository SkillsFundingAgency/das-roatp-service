#nullable enable

using SFA.DAS.RoATPService.Application.Api.Serialization;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Application.Api.Models;

public class PatchOrganisationModel
{
    public Optional<ProviderType?> ProviderType { get; set; }
    public Optional<int> OrganisationTypeId { get; set; }
    public Optional<OrganisationStatus> Status { get; set; }
    public Optional<string> ReasonForRemoval { get; set; }
}
