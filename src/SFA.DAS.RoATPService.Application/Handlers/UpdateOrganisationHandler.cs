using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SFA.DAS.RoATPService.Application.Commands;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Exceptions;
    using Validators;

    public class UpdateOrganisationHandler : IRequestHandler<UpdateOrganisationRequest, bool>
    {
        private readonly ILogger<UpdateOrganisationHandler> _logger;
        private readonly IOrganisationValidator _validator;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;
        private readonly IAuditLogService _auditLogService;
        private readonly IOrganisationValidator _organisationValidator;
        private readonly IProviderTypeValidator _providerTypeValidator;
        private readonly ITextSanitiser _textSanitiser;

        private const string FieldChanged = "Application Determined Date";

        public UpdateOrganisationHandler(ILogger<UpdateOrganisationHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository, IAuditLogService auditLogService, IOrganisationValidator organisationValidator, IProviderTypeValidator providerTypeValidator, ITextSanitiser textSanitiser)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;
            _auditLogService = auditLogService;
            _organisationValidator = organisationValidator;
            _providerTypeValidator = providerTypeValidator;
            _textSanitiser = textSanitiser;
        }

        public async Task<bool> Handle(UpdateOrganisationRequest request, CancellationToken cancellationToken)
        {
            request.LegalName = _textSanitiser.SanitiseInputText(request.LegalName);
            request.TradingName = _textSanitiser.SanitiseInputText(request.TradingName);

            ValidateOrganisation(request);

            _logger.LogInformation($@"Handling Update organisation for Organisation ID [{request.OrganisationId}]");

            var auditChanges = BuildAuditDetails(request);

            if (!auditChanges.Any()) return true;
        
            var command = new UpdateOrganisationCommand
            {
                OrganisationId = request.OrganisationId,
                OrganisationTypeId = request.OrganisationTypeId,
                ApplicationDeterminedDate = request.ApplicationDeterminedDate,
                CharityNumber = request.CharityNumber,
                CompanyNumber = request.CompanyNumber,
                LegalName = request.LegalName,
                ProviderTypeId = request.ProviderTypeId,
                TradingName = request.TradingName,
                Username = request.Username
            };

            var success = await _updateOrganisationRepository.UpdateOrganisation(command);

            if (success)
            {
                    
                foreach (var audit in auditChanges)
                {
                    await _updateOrganisationRepository.WriteFieldChangesToAuditLog(audit);
                }

                return true;
            }
               
            return false;

        }

        private List<AuditData>  BuildAuditDetails(UpdateOrganisationRequest request)
        {
            var auditChanges = new List<AuditData>();
            var auditChangesMade = _auditLogService.AuditLegalName(request.OrganisationId, request.Username, request.LegalName);

            if (auditChangesMade.ChangesMade)
            {
                auditChanges.Add(auditChangesMade);
            }

            auditChangesMade = _auditLogService.AuditTradingName(request.OrganisationId, request.Username, request.TradingName);
            if (auditChangesMade.ChangesMade)
                auditChanges.Add(auditChangesMade);
            //
            // auditChangesMade =
            //     _auditLogService.AuditOrganisationType(request.OrganisationId, request.Username, request.OrganisationTypeId);
            // if (auditChangesMade.ChangesMade)
            //     auditChanges.Add(auditChangesMade);

            auditChangesMade = _auditLogService.AuditProviderType(request.OrganisationId, request.Username,
                request.ProviderTypeId, request.OrganisationTypeId);
            if (auditChangesMade.ChangesMade)
                auditChanges.Add(auditChangesMade);

            auditChangesMade =
                _auditLogService.AuditCompanyNumber(request.OrganisationId, request.Username, request.CompanyNumber);
            if (auditChangesMade.ChangesMade)
                auditChanges.Add(auditChangesMade);

            auditChangesMade =
                _auditLogService.AuditCharityNumber(request.OrganisationId, request.Username, request.CharityNumber);
            if (auditChangesMade.ChangesMade)
                auditChanges.Add(auditChangesMade);

            auditChangesMade = _auditLogService.AuditApplicationDeterminedDate(request.OrganisationId, request.Username,
                request.ApplicationDeterminedDate);
            if (auditChangesMade.ChangesMade)
                auditChanges.Add(auditChangesMade);

            return auditChanges;
            
        }

        private void ValidateOrganisation(UpdateOrganisationRequest request)
        {
            if (!IsValidUpdateOrganisation(request))
            {
                var invalidOrganisationError = "Invalid Organisation data";
                if (!_organisationValidator.IsValidLegalName(request.LegalName))
                    invalidOrganisationError = $"{invalidOrganisationError}: Invalid Legal Name [{request.LegalName}]";

                if (!_organisationValidator.IsValidTradingName(request.TradingName))
                    invalidOrganisationError = $"{invalidOrganisationError}: Invalid Trading Name [{request.TradingName}]";

                if (!_providerTypeValidator.IsValidProviderTypeId(request.ProviderTypeId))
                    invalidOrganisationError =
                        $"{invalidOrganisationError}: Invalid Provider Type Id [{request.ProviderTypeId}]";

                if (!_organisationValidator.IsValidOrganisationTypeId(request.OrganisationTypeId))
                    invalidOrganisationError =
                        $"{invalidOrganisationError}: Invalid Organisation Type Id [{request.OrganisationTypeId}]";


                if (!_organisationValidator.IsValidCompanyNumber(request.CompanyNumber))
                    invalidOrganisationError = $"{invalidOrganisationError}: Invalid company number [{request.CompanyNumber}]";

                if (!_organisationValidator.IsValidApplicationDeterminedDate(request.ApplicationDeterminedDate))
                    invalidOrganisationError =
                        $"{invalidOrganisationError}: Invalid Application Determined Date [{request.ApplicationDeterminedDate}]";

                if (!_organisationValidator.IsValidCharityNumber(request.CharityNumber))
                    invalidOrganisationError =
                        $"{invalidOrganisationError}: Invalid charity registration number [{request.CharityNumber}]";

                _logger.LogInformation(invalidOrganisationError);
                throw new BadRequestException(invalidOrganisationError);
            }
        }


        private bool IsValidUpdateOrganisation(UpdateOrganisationRequest request)
        {
            return (_organisationValidator.IsValidLegalName(request.LegalName)
                    && _organisationValidator.IsValidTradingName(request.TradingName)
                    && _providerTypeValidator.IsValidProviderTypeId(request.ProviderTypeId)
                    && _organisationValidator.IsValidOrganisationTypeId(request.OrganisationTypeId)
                    && _organisationValidator.IsValidApplicationDeterminedDate(request.ApplicationDeterminedDate)
                    && _organisationValidator.IsValidCompanyNumber(request.CompanyNumber)
                    && _organisationValidator.IsValidCharityNumber(request.CharityNumber));
        }
    }
}