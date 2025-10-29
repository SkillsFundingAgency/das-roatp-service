using System;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch.Operations;
using SFA.DAS.RoATPService.Application.Common.Validators;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.Commands.PatchOrganisation;

public class PatchOrganisationCommandValidator : AbstractValidator<PatchOrganisationCommand>
{
    public const string RequestingUserIdIsRequiredErrorMessage = "RequestingUserId is required.";
    public const string PatchDocCannotBeEmptyErrorMessage = "At least one patch operation is required.";
    public const string PatchDocUnexpectedPathErrorMessage = "Unexpected path in patch operations.";
    public const string PatchDocOnlyReplaceOperationExpectedErrorMessage = "Only replace operation is expected.";
    public const string PatchDocDuplicatePathNotAllowedErrorMessage = "Duplicate operation paths are not allowed.";
    public const string PatchDocStatusShouldBeValidErrorMessage = "If path is '/status', value must be a valid OrganisationStatus enum value (name or int).";
    public const string PatchDocProviderTypeShouldBeValidErrorMessage = "If path is '/providerType', value must be a valid ProviderType enum value (name or int).";
    public const string PatchDocRemovedStatusShouldAccompanyRemovedReasonIdErrorMessage = "If status is being change to Removed, removedReasonId operation is required.";
    public const string PatchDocOtherThanRemovedStatusShouldNotHaveRemovedReasonOperationErrorMessage = "If status is being change to any status other than Removed, removedReasonId operation should not be given.";
    public const string PatchDocRemovedReasonIdShouldBeValidErrorMessage = "Invalid RemovedReasonId.";
    public const string PatchDocOrganisationTypeIdShouldBeValidErrorMessage = "Invalid OrganisationTypeId.";
    public const string PatchDocRemovedReasonIdIsNotValidIfOrganisationStatusIsNotRemovedErrorMessage = "Invalid request to update RemovedReasonId when organisation status is not removed";

    public PatchOrganisationCommandValidator(IRemovedReasonsRepository removedReasonsRepository, IOrganisationTypesRepository organisationTypesRepository, IOrganisationsRepository organisationsRepository)
    {
        RuleFor(x => x.Ukprn)
            .Cascade(CascadeMode.Stop)
            .UkprnNotEmpty()
            .MustBeValidUkprnFormat();

        RuleFor(x => x.PatchDoc.Operations).NotEmpty().WithMessage(PatchDocCannotBeEmptyErrorMessage);

        RuleForEach(c => c.PatchDoc.Operations)
            .ChildRules(operation =>
            {
                operation.RuleFor(o => PatchOrganisationCommandValidatorHelper.PatchFields.Contains(o.path.ToLowerInvariant())).Equal(true).WithMessage(PatchDocUnexpectedPathErrorMessage);
            });

        RuleForEach(x => x.PatchDoc.Operations)
            .ChildRules(operation =>
            {
                operation.RuleFor(o => o.OperationType == OperationType.Replace).Equal(true).WithMessage(PatchDocOnlyReplaceOperationExpectedErrorMessage);
            });

        RuleFor(x => x.PatchDoc.Operations)
            .Must(ops =>
            {
                var paths = ops.Select(o => o.path?.ToLowerInvariant()).Where(p => p != null).ToList();
                return paths.Count == paths.Distinct().Count();
            })
            .WithMessage(PatchDocDuplicatePathNotAllowedErrorMessage);

        RuleFor(x => x.PatchDoc.Operations)
            .Must(ops =>
            {
                var statusOp = ops.FindOperationWithPath(PatchOrganisationCommandValidatorHelper.StatusPath);
                if (statusOp == null) return true;

                return EnumValidationHelper.IsValidEnumValue<OrganisationStatus>(statusOp.value);
            })
            .WithMessage(PatchDocStatusShouldBeValidErrorMessage);

        RuleFor(x => x.PatchDoc.Operations)
            .Must(ops =>
            {
                var statusOp = ops.FindOperationWithPath(PatchOrganisationCommandValidatorHelper.ProviderTypePath);
                if (statusOp == null) return true;

                return statusOp.value.IsValidEnumValue<ProviderType>();
            })
            .WithMessage(PatchDocProviderTypeShouldBeValidErrorMessage);

        RuleFor(x => x.PatchDoc.Operations)
            .Must(ops =>
            {
                var statusOp = ops.FindOperationWithPath(PatchOrganisationCommandValidatorHelper.StatusPath);
                if (statusOp == null) return true;

                if (Enum.TryParse<OrganisationStatus>(statusOp.value.ToString(), out var status) && status == OrganisationStatus.Removed)
                {
                    var removedReasonOp = ops.FindOperationWithPath(PatchOrganisationCommandValidatorHelper.RemovedReasonIdPath);
                    return (removedReasonOp != null);
                }

                return true;
            })
            .WithMessage(PatchDocRemovedStatusShouldAccompanyRemovedReasonIdErrorMessage);

        RuleFor(x => x.PatchDoc.Operations)
            .Must(ops =>
            {
                var statusOp = ops.FindOperationWithPath(PatchOrganisationCommandValidatorHelper.StatusPath);
                if (statusOp == null) return true;

                if (Enum.TryParse<OrganisationStatus>(statusOp.value.ToString(), out var status) && status != OrganisationStatus.Removed)
                {
                    var removedReasonOp = ops.FindOperationWithPath(PatchOrganisationCommandValidatorHelper.RemovedReasonIdPath);
                    return (removedReasonOp == null);
                }

                return true;
            })
            .WithMessage(PatchDocOtherThanRemovedStatusShouldNotHaveRemovedReasonOperationErrorMessage);

        RuleFor(x => x.PatchDoc.Operations)
            .MustAsync(async (command, ops, cancellationToken) =>
            {
                var removedReasonOp = ops.FindOperationWithPath(PatchOrganisationCommandValidatorHelper.RemovedReasonIdPath);
                var statusOp = ops.FindOperationWithPath(PatchOrganisationCommandValidatorHelper.StatusPath);
                if (removedReasonOp != null && statusOp == null)
                {
                    var org = await organisationsRepository.GetOrganisationByUkprn(command.Ukprn, cancellationToken);
                    return org != null && org.Status == OrganisationStatus.Removed;
                }
                return true;
            })
            .WithMessage(PatchDocRemovedReasonIdIsNotValidIfOrganisationStatusIsNotRemovedErrorMessage);

        RuleFor(x => x.PatchDoc.Operations)
            .MustAsync(async (ops, cancellationToken) =>
            {
                var removedReasonOp = ops.FindOperationWithPath(PatchOrganisationCommandValidatorHelper.RemovedReasonIdPath);
                if (removedReasonOp == null) return true;

                var reasons = await removedReasonsRepository.GetAllRemovedReasons(cancellationToken);
                return reasons.Select(r => r.Id.ToString()).Contains(removedReasonOp.value.ToString());
            })
            .WithMessage(PatchDocRemovedReasonIdShouldBeValidErrorMessage);

        RuleFor(x => x.PatchDoc.Operations)
            .MustAsync(async (ops, cancellationToken) =>
            {
                var organisationTypeIdOp = ops.FindOperationWithPath(PatchOrganisationCommandValidatorHelper.OrganisationTypeIdPath);
                if (organisationTypeIdOp == null) return true;

                var organisationTypes = await organisationTypesRepository.GetOrganisationTypes(cancellationToken);
                return organisationTypes.Select(r => r.Id.ToString()).Contains(organisationTypeIdOp.value.ToString());
            })
            .WithMessage(PatchDocOrganisationTypeIdShouldBeValidErrorMessage);
    }
}
