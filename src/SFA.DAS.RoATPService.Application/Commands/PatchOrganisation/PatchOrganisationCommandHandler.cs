using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.RoATPService.Application.Common.Models;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Domain;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.Commands.PatchOrganisation;

public class PatchOrganisationCommandHandler(IOrganisationsRepository _organisationRepository) : IRequestHandler<PatchOrganisationCommand, ValidatedResponse<SuccessModel>>
{
    public async Task<ValidatedResponse<SuccessModel>> Handle(PatchOrganisationCommand request, CancellationToken cancellationToken)
    {
        var organisation = await _organisationRepository.GetOrganisationByUkprn(request.Ukprn, cancellationToken);

        if (organisation == null) return new ValidatedResponse<SuccessModel>(new SuccessModel(false));

        var patchModel = (PatchOrganisationModel)organisation;

        request.PatchDoc.ApplyTo(patchModel);

        var auditRecord = GetAuditRecord(organisation, patchModel, request.UserId);

        organisation.Status = patchModel.Status;
        organisation.RemovedReasonId = patchModel.RemovedReasonId;
        organisation.ProviderType = patchModel.ProviderType;
        organisation.OrganisationTypeId = patchModel.OrganisationTypeId;
        organisation.UpdatedBy = request.UserId;
        organisation.UpdatedAt = DateTime.UtcNow;

        await _organisationRepository.UpdateOrganisation(organisation, auditRecord, cancellationToken);

        return new ValidatedResponse<SuccessModel>(new SuccessModel(true));
    }

    private Audit GetAuditRecord(Domain.Entities.Organisation organisation, PatchOrganisationModel patchOrganisationModel, string userId)
    {
        var auditData = new AuditData { FieldChanges = [], OrganisationId = organisation.Id, UpdatedAt = DateTime.Now, UpdatedBy = userId };
        if (organisation.Status != patchOrganisationModel.Status)
        {
            auditData.FieldChanges.Add(new AuditLogEntry
            {
                FieldChanged = AuditLogField.OrganisationStatus,
                PreviousValue = organisation.Status.ToString(),
                NewValue = patchOrganisationModel.Status.ToString()
            });
        }

        if (organisation.RemovedReasonId != patchOrganisationModel.RemovedReasonId)
        {
            auditData.FieldChanges.Add(new AuditLogEntry
            {
                FieldChanged = AuditLogField.RemovedReason,
                PreviousValue = organisation.RemovedReasonId?.ToString() ?? "null",
                NewValue = patchOrganisationModel.RemovedReasonId?.ToString() ?? "null"
            });
        }

        if (organisation.ProviderType != patchOrganisationModel.ProviderType)
        {
            auditData.FieldChanges.Add(new AuditLogEntry
            {
                FieldChanged = AuditLogField.ProviderType,
                PreviousValue = organisation.ProviderType.ToString(),
                NewValue = patchOrganisationModel.ProviderType.ToString()
            });
        }

        if (organisation.OrganisationTypeId != patchOrganisationModel.OrganisationTypeId)
        {
            auditData.FieldChanges.Add(new AuditLogEntry
            {
                FieldChanged = AuditLogField.OrganisationType,
                PreviousValue = organisation.OrganisationTypeId.ToString(),
                NewValue = patchOrganisationModel.OrganisationTypeId.ToString()
            });
        }

        return new Audit
        {
            OrganisationId = organisation.Id,
            UpdatedBy = userId,
            UpdatedAt = System.DateTime.UtcNow,
            AuditData = auditData
        };
    }
}
