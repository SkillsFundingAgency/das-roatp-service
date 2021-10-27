using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using SFA.DAS.RoATPService.Application.Commands;
using SFA.DAS.RoATPService.Application.Types;
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
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;
        private readonly IAuditLogService _auditLogService;
        private readonly IOrganisationValidator _organisationValidator;
        private readonly ITextSanitiser _textSanitiser;

        public UpdateOrganisationHandler(ILogger<UpdateOrganisationHandler> logger, IUpdateOrganisationRepository updateOrganisationRepository, IAuditLogService auditLogService, IOrganisationValidator organisationValidator,  ITextSanitiser textSanitiser)
        {
            _logger = logger;
            _updateOrganisationRepository = updateOrganisationRepository;
            _auditLogService = auditLogService;
            _organisationValidator = organisationValidator;
            _textSanitiser = textSanitiser;
        }

        public async Task<bool> Handle(UpdateOrganisationRequest request, CancellationToken cancellationToken)
        {
            request.LegalName = _textSanitiser.SanitiseInputText(request.LegalName);
            request.TradingName = _textSanitiser.SanitiseInputText(request.TradingName);

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

            _logger.LogInformation($@"Handling Update organisation for Organisation ID [{request.OrganisationId}]");

            var errorMessage = _organisationValidator.ValidateOrganisation(command);

            if (!string.IsNullOrEmpty(errorMessage?.Message))
                throw new BadRequestException(errorMessage.Message);

            var auditChanges = _auditLogService.AuditOrganisation(command);

            if (auditChanges is null) return true;
            
            var success = await _updateOrganisationRepository.UpdateOrganisation(command);

            if (!success) return false;
            
            await _updateOrganisationRepository.WriteFieldChangesToAuditLog(auditChanges);
            

            return true;
        }
    }
}