using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using SFA.DAS.RoATPService.Application.Common;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.Commands.PatchOrganisation;

public record PatchOrganisationCommand(int Ukprn, string UserId, JsonPatchDocument<PatchOrganisationModel> PatchDoc)
    : IRequest<ValidatedResponse>;
public class PatchOrganisationModel
{
    public OrganisationStatus Status { get; set; }
    public int? RemovedReasonId { get; set; }
    public ProviderType ProviderType { get; set; }
    public int OrganisationTypeId { get; set; }

    public static implicit operator PatchOrganisationModel(Organisation organisation) =>
        new PatchOrganisationModel
        {
            Status = organisation.Status,
            RemovedReasonId = organisation.RemovedReasonId,
            ProviderType = organisation.ProviderType,
            OrganisationTypeId = organisation.OrganisationTypeId
        };
};

public class PatchOrganisationCommendHandler(IOrganisationsRepository _organisationRepository) : IRequestHandler<PatchOrganisationCommand, ValidatedResponse>
{
    public async Task<ValidatedResponse> Handle(PatchOrganisationCommand request, CancellationToken cancellationToken)
    {
        var organisation = await _organisationRepository.GetOrganisationByUkprn(request.Ukprn, cancellationToken);

        if (organisation == null) return null;

        var patchModel = (PatchOrganisationModel)organisation;

        request.PatchDoc.ApplyTo(patchModel);

        organisation.Status = patchModel.Status;
        organisation.RemovedReasonId = patchModel.RemovedReasonId;
        organisation.ProviderType = patchModel.ProviderType;
        organisation.OrganisationTypeId = patchModel.OrganisationTypeId;
        organisation.UpdatedBy = request.UserId;
        organisation.UpdatedAt = System.DateTime.UtcNow;

        await _organisationRepository.UpdateOrganisation(organisation, [], cancellationToken);

        return new ValidatedResponse();
    }
}

public class PatchOrganisationCommandValidator : AbstractValidator<PatchOrganisationCommand>
{
    public const string StatusPath = "/status";
    public const string ProviderTypePath = "/providerType";
    public const string OrganisationTypeIdPath = "/organisationTypeId";
    public const string RemovedReasonIdPath = "/removedReasonId";
    public static readonly List<string> PatchFields =
    [
        StatusPath, ProviderTypePath, OrganisationTypeIdPath, RemovedReasonIdPath
    ];

    public PatchOrganisationCommandValidator(IRemovedReasonsRepository removedReasonsRepository, IOrganisationTypesRepository organisationTypesRepository, IOrganisationsRepository organisationsRepository)
    {
        RuleFor(x => x.Ukprn).GreaterThan(0).WithMessage("Ukprn must be greater than 0");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("X-RequestingUserId header is required");
        RuleFor(x => x.PatchDoc.Operations).NotEmpty().WithMessage("At least one patch operation is required").OverridePropertyName("Operations");
        RuleForEach(c => c.PatchDoc.Operations)
            .ChildRules(operation =>
            {
                operation.RuleFor(o => PatchFields.Contains(o.path)).Equal(true).WithMessage("Invalid path in patch operations");
            });
        RuleForEach(x => x.PatchDoc.Operations)
            .ChildRules(operation =>
            {
                operation.RuleFor(o => o.OperationType).Equal(OperationType.Replace).WithMessage("Only replace operation is expected");
            });
        RuleFor(x => x.PatchDoc.Operations)
            .Must(ops =>
            {
                var paths = ops.Select(o => o.path?.ToLowerInvariant()).Where(p => p != null).ToList();
                return paths.Count == paths.Distinct().Count();
            })
            .WithMessage("Duplicate operation paths are not allowed")
            .OverridePropertyName("Operations");
        RuleFor(x => x.PatchDoc.Operations)
            .Must(ops =>
            {
                var statusOp = ops.Find(o => o.path == StatusPath);
                if (statusOp == null) return true;

                return EnumValidationHelper.IsValidEnumValue<OrganisationStatus>(statusOp.value);
            })
            .WithMessage("If path is '/status', value must be a valid OrganisationStatus enum value (name or int).")
            .OverridePropertyName("/status Operation");
        RuleFor(x => x.PatchDoc.Operations)
            .Must(ops =>
            {
                var statusOp = ops.Find(o => o.path == ProviderTypePath);
                if (statusOp == null) return true;

                return statusOp.value.IsValidEnumValue<ProviderType>();
            })
            .WithMessage("If path is '/providerType', value must be a valid ProviderType enum value (name or int).")
            .OverridePropertyName("/providerType Operation");
        RuleFor(x => x.PatchDoc.Operations)
            .Must(ops =>
            {
                var statusOp = ops.Find(o => o.path == StatusPath);
                if (statusOp == null) return true;

                if (Enum.TryParse<OrganisationStatus>(statusOp.value.ToString(), out var status) && status == OrganisationStatus.Removed)
                {
                    var removedReasonOp = ops.Find(o => o.path == RemovedReasonIdPath);
                    return (removedReasonOp != null);
                }

                return true;
            })
            .WithMessage("If status is being change to Removed, removedReasonId operation is required.")
            .OverridePropertyName("/status Operation");

        RuleFor(x => x.PatchDoc.Operations)
            .Must(ops =>
            {
                var statusOp = ops.Find(o => o.path == StatusPath);
                if (statusOp == null) return true;

                if (Enum.TryParse<OrganisationStatus>(statusOp.value.ToString(), out var status) && status != OrganisationStatus.Removed)
                {
                    var removedReasonOp = ops.Find(o => o.path == RemovedReasonIdPath);
                    return (removedReasonOp == null);
                }

                return true;
            })
            .WithMessage("If status is being change to any status other than Removed, removedReasonId operation should not be given.")
            .OverridePropertyName("/status Operation");

        RuleFor(x => x.PatchDoc.Operations)
            .MustAsync(async (ops, cancellationToken) =>
            {
                var removedReasonOp = ops.Find(o => o.path == RemovedReasonIdPath);
                if (removedReasonOp == null) return true;

                var reasons = await removedReasonsRepository.GetAllRemovedReasons(cancellationToken);
                return reasons.Select(r => r.Id.ToString()).Contains(removedReasonOp.value.ToString());
            })
            .WithMessage("Invalid RemovedReasonId.")
            .OverridePropertyName("/removedReasonId Operation");

        RuleFor(x => x)
            .MustAsync(async (request, cancellationToken) =>
            {
                var removedReasonOp = request.PatchDoc.Operations.Find(o => o.path == RemovedReasonIdPath);
                var statusOp = request.PatchDoc.Operations.Find(o => o.path == StatusPath);
                if (removedReasonOp != null && statusOp == null)
                {
                    var org = await organisationsRepository.GetOrganisationByUkprn(request.Ukprn, cancellationToken);
                    return org != null && org.Status == OrganisationStatus.Removed;
                }
                return true;
            })
            .WithMessage("Invalid request to update RemovedReasonId when organisation status is not removed")
            .OverridePropertyName("/removedReasonId Operation");

        RuleFor(x => x.PatchDoc.Operations)
            .MustAsync(async (ops, cancellationToken) =>
            {
                var organisationTypeIdOp = ops.Find(o => o.path == OrganisationTypeIdPath);
                if (organisationTypeIdOp == null) return true;

                var organisationTypes = await organisationTypesRepository.GetOrganisationTypes(cancellationToken);
                return organisationTypes.Select(r => r.Id.ToString()).Contains(organisationTypeIdOp.value.ToString());
            })
            .WithMessage("Invalid OrganisationTypeId.")
            .OverridePropertyName("/organisationTypeId Operation");
    }
}
