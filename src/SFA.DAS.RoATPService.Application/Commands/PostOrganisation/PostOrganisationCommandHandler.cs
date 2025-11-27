using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.RoATPService.Application.Common.Models;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.RoATPService.Domain;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using OrganisationStatus = SFA.DAS.RoATPService.Domain.Common.OrganisationStatus;
using ProviderType = SFA.DAS.RoATPService.Domain.Common.ProviderType;

namespace SFA.DAS.RoATPService.Application.Commands.PostOrganisation;

public class PostOrganisationCommandHandler(IOrganisationsRepository organisationsRepository, ITextSanitiser textSanitiser) : IRequestHandler<PostOrganisationCommand, ValidatedResponse<SuccessModel>>
{
    public async Task<ValidatedResponse<SuccessModel>> Handle(PostOrganisationCommand command, CancellationToken cancellationToken)
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
            LegalName = textSanitiser.SanitiseInputText(command.LegalName),
            TradingName = textSanitiser.SanitiseInputText(command.TradingName),
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

        return new ValidatedResponse<SuccessModel>(new SuccessModel(true));
    }
}
