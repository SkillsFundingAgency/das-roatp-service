using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.RoATPService.Application.Common.Models;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.RoATPService.Domain.AuditModels;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using OrganisationStatus = SFA.DAS.RoATPService.Domain.Common.OrganisationStatus;
using ProviderType = SFA.DAS.RoATPService.Domain.Common.ProviderType;

namespace SFA.DAS.RoATPService.Application.Commands.UpsertOrganisation;

public class UpsertOrganisationCommandHandler(IOrganisationsRepository organisationsRepository) : IRequestHandler<UpsertOrganisationCommand, ValidatedResponse<SuccessModel>>
{
    public async Task<ValidatedResponse<SuccessModel>> Handle(UpsertOrganisationCommand command, CancellationToken cancellationToken)
    {
        Task task = command.IsNewOrganisation ? CreateOrganisation(command, cancellationToken) : UpdateOrganisation(command, cancellationToken);

        await task;

        return new ValidatedResponse<SuccessModel>(new SuccessModel(true));
    }

    private async Task CreateOrganisation(UpsertOrganisationCommand command, CancellationToken cancellationToken)
    {
        var organisationId = Guid.NewGuid();

        var auditRecord = new Audit
        {
            OrganisationId = organisationId,
            UpdatedBy = command.RequestingUserId,
            UpdatedAt = DateTime.UtcNow,
            AuditData = new AuditData
            {
                FieldChanges = [],
                OrganisationId = organisationId,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = command.RequestingUserId
            }
        };

        var isSupportingProvider = command.ProviderType.ToString() == ProviderType.Supporting.ToString();

        var organisationData = new Domain.Entities.OrganisationData
        {
            CompanyNumber = command.CompanyNumber?.ToUpper(),
            CharityNumber = command.CharityNumber,
            StartDate = isSupportingProvider ? DateTime.UtcNow : null,
            ApplicationDeterminedDate = DateTime.UtcNow
        };

        var organisation = new Domain.Entities.Organisation
        {
            Id = organisationId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = command.RequestingUserId,
            Status = isSupportingProvider ? OrganisationStatus.Active : OrganisationStatus.OnBoarding,
            ProviderType = command.ProviderType,
            OrganisationTypeId = command.OrganisationTypeId!.Value,
            Ukprn = command.Ukprn,
            LegalName = command.LegalName,
            TradingName = command.TradingName,
            StatusDate = DateTime.UtcNow,
            OrganisationData = organisationData,
            CompanyNumber = command.CompanyNumber,
            CharityNumber = command.CharityNumber,
            StartDate = isSupportingProvider ? DateTime.UtcNow : null,
            ApplicationDeterminedDate = DateTime.UtcNow
        };

        OrganisationStatusEvent statusEvent = new()
        {
            CreatedOn = DateTime.UtcNow,
            OrganisationStatus = organisation.Status,
            Ukprn = organisation.Ukprn,
        };

        await organisationsRepository.CreateOrganisation(organisation, auditRecord, statusEvent, cancellationToken);
    }

    private async Task UpdateOrganisation(UpsertOrganisationCommand command, CancellationToken cancellationToken)
    {
        Domain.Entities.Organisation organisation = await organisationsRepository.GetOrganisationByUkprn(command.Ukprn, cancellationToken);
        if (organisation == null)
        {
            throw new InvalidOperationException($"Organisation with UKPRN {command.Ukprn} not found.");
        }

        Audit auditRecord = GetAuditRecord(organisation, command);

        organisation.ProviderType = command.ProviderType;
        organisation.OrganisationTypeId = command.OrganisationTypeId!.Value;
        organisation.ApplicationDeterminedDate = DateTime.UtcNow.Date;
        organisation.LegalName = command.LegalName;
        organisation.TradingName = command.TradingName;
        organisation.CompanyNumber = command.CompanyNumber?.ToUpper();
        organisation.CharityNumber = command.CharityNumber;
        organisation.UpdatedAt = DateTime.UtcNow;
        organisation.UpdatedBy = command.RequestingUserId;

        await organisationsRepository.UpdateOrganisation(organisation, auditRecord, null, cancellationToken);
    }

    private static Audit GetAuditRecord(Domain.Entities.Organisation organisation, UpsertOrganisationCommand command)
    {
        var auditData = new AuditData
        {
            FieldChanges = [],
            OrganisationId = organisation.Id,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = command.RequestingUserId
        };

        if (organisation.LegalName != command.LegalName)
        {
            auditData.FieldChanges.Add(new AuditLogEntry
            {
                FieldChanged = AuditLogFields.LegalName,
                PreviousValue = organisation.LegalName,
                NewValue = command.LegalName
            });
        }

        if (organisation.TradingName != command.TradingName)
        {
            auditData.FieldChanges.Add(new AuditLogEntry
            {
                FieldChanged = AuditLogFields.TradingName,
                PreviousValue = organisation.TradingName,
                NewValue = command.TradingName
            });
        }

        if (organisation.CompanyNumber != command.CompanyNumber)
        {
            auditData.FieldChanges.Add(new AuditLogEntry
            {
                FieldChanged = AuditLogFields.CompanyNumber,
                PreviousValue = organisation.CompanyNumber,
                NewValue = command.CompanyNumber
            });
        }

        if (organisation.CharityNumber != command.CharityNumber)
        {
            auditData.FieldChanges.Add(new AuditLogEntry
            {
                FieldChanged = AuditLogFields.CharityNumber,
                PreviousValue = organisation.CharityNumber,
                NewValue = command.CharityNumber
            });
        }

        if (organisation.ProviderType != command.ProviderType)
        {
            auditData.FieldChanges.Add(new AuditLogEntry
            {
                FieldChanged = AuditLogFields.ProviderType,
                PreviousValue = organisation.ProviderType.ToString(),
                NewValue = command.ProviderType.ToString()
            });
        }

        if (organisation.OrganisationTypeId != command.OrganisationTypeId)
        {
            auditData.FieldChanges.Add(new AuditLogEntry
            {
                FieldChanged = AuditLogFields.OrganisationType,
                PreviousValue = organisation.OrganisationTypeId.ToString(),
                NewValue = command.OrganisationTypeId.ToString()
            });
        }

        return new Audit
        {
            OrganisationId = organisation.Id,
            UpdatedBy = command.RequestingUserId,
            UpdatedAt = DateTime.UtcNow,
            AuditData = auditData
        };
    }
}
