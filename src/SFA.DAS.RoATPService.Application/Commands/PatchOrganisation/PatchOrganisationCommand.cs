using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using SFA.DAS.RoATPService.Application.Common.Models;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.RoATPService.Domain.Common;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Application.Commands.PatchOrganisation;

public record PatchOrganisationCommand(int Ukprn, string UserId, JsonPatchDocument<PatchOrganisationModel> PatchDoc)
    : IRequest<ValidatedResponse<SuccessModel>>;

public class PatchOrganisationModel
{
    public OrganisationStatus Status { get; set; }
    public int? RemovedReasonId { get; set; }
    public Domain.Common.ProviderType ProviderType { get; set; }
    public int OrganisationTypeId { get; set; }

    public static implicit operator PatchOrganisationModel(Organisation organisation) =>
        new PatchOrganisationModel
        {
            Status = organisation.Status,
            RemovedReasonId = organisation.RemovedReasonId,
            ProviderType = (Domain.Common.ProviderType)organisation.ProviderTypeId,
            OrganisationTypeId = organisation.OrganisationTypeId
        };
};
