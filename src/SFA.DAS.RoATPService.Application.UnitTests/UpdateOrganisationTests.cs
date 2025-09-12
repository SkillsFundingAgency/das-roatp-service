using SFA.DAS.RoATPService.Application.Commands;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Application.Types;

namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Domain;
    using Exceptions;
    using FluentAssertions;
    using Handlers;
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using Validators;

    [TestFixture]
    public class UpdateOrganisationTests
    {
        private Mock<ILogger<UpdateOrganisationHandler>> _logger;
        private Mock<IOrganisationValidator> _organisationValidator;
        private Mock<IUpdateOrganisationRepository> _updateOrganisationRepository;
        private UpdateOrganisationHandler _handler;
        private UpdateOrganisationRequest _request;
        private Mock<IAuditLogService> _auditLogService;
        private ITextSanitiser _textSanitiser;
        private ValidationErrorMessage _validationErrorMessage;
        private AuditData _auditData;

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<UpdateOrganisationHandler>>();
            _organisationValidator = new Mock<IOrganisationValidator>();
            ;
            _textSanitiser = new TextSanitiser();
            _validationErrorMessage = new ValidationErrorMessage();

            _updateOrganisationRepository = new Mock<IUpdateOrganisationRepository>();
            _auditLogService = new Mock<IAuditLogService>();

            _organisationValidator.Setup(x => x.ValidateOrganisation(It.IsAny<UpdateOrganisationCommand>()))
                .Returns(_validationErrorMessage);

            _auditData = new AuditData { };

            _auditLogService.Setup(x => x.AuditOrganisation(It.IsAny<UpdateOrganisationCommand>())).Returns(_auditData);

            _handler = new UpdateOrganisationHandler(_logger.Object, _updateOrganisationRepository.Object, _auditLogService.Object, _organisationValidator.Object, _textSanitiser);
            _request = new UpdateOrganisationRequest
            {
                OrganisationId = Guid.NewGuid(),
                OrganisationTypeId = 1,
                ProviderTypeId = 1,
                LegalName = "legal name",
                CharityNumber = "1233333",
                CompanyNumber = "35444444",
                Username = "john smith",
                ApplicationDeterminedDate = DateTime.Today
            };
        }


        [Test]
        public void Handler_updates_organisation_and_records_audit_history()
        {
            _updateOrganisationRepository.Setup(x =>
                    x.UpdateOrganisation(It.IsAny<UpdateOrganisationCommand>()))
                .ReturnsAsync(true).Verifiable();

            _updateOrganisationRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()))
                .ReturnsAsync(true).Verifiable();

            var result = _handler.Handle(_request, new CancellationToken()).Result;
            result.Should().BeTrue();
            _updateOrganisationRepository.VerifyAll();
            _auditLogService.Verify(x => x.AuditOrganisation(It.IsAny<UpdateOrganisationCommand>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.UpdateOrganisation(It.IsAny<UpdateOrganisationCommand>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Once);
        }

        [Test]
        public async Task Handler_rejects_request_with_invalid_organisation()
        {
            var validationMessage = new ValidationErrorMessage { Message = "error" };
            _organisationValidator.Setup(x => x.ValidateOrganisation(It.IsAny<UpdateOrganisationCommand>()))
                .Returns(validationMessage);
            _organisationValidator.Setup(x => x.IsValidOrganisationTypeId(It.IsAny<int>())).Returns(false);

            Func<Task> result = () => _handler.Handle(_request, new CancellationToken());
            await result.Should().ThrowAsync<BadRequestException>();
        }

        [Test]
        public void Handler_not_update_organisation_if_no_changes()
        {
            _auditLogService.Setup(x => x.AuditOrganisation(It.IsAny<UpdateOrganisationCommand>())).Returns((AuditData)null);

            var result = _handler.Handle(_request, new CancellationToken()).Result;
            result.Should().BeTrue();
            _updateOrganisationRepository.VerifyAll();
            _auditLogService.Verify(x => x.AuditOrganisation(It.IsAny<UpdateOrganisationCommand>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.UpdateOrganisation(It.IsAny<UpdateOrganisationCommand>()), Times.Never);
            _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [Test]
        public void Handler_not_update_organisation_if_audit_fails()
        {
            _auditLogService.Setup(x => x.AuditOrganisation(It.IsAny<UpdateOrganisationCommand>())).Returns(_auditData);
            _updateOrganisationRepository.Setup(x =>
                    x.UpdateOrganisation(It.IsAny<UpdateOrganisationCommand>()))
                .ReturnsAsync(false).Verifiable();

            var result = _handler.Handle(_request, new CancellationToken()).Result;
            result.Should().BeFalse();
            _updateOrganisationRepository.VerifyAll();
            _auditLogService.Verify(x => x.AuditOrganisation(It.IsAny<UpdateOrganisationCommand>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.UpdateOrganisation(It.IsAny<UpdateOrganisationCommand>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

    }
}
