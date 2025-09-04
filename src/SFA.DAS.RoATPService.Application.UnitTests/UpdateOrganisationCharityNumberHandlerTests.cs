﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Exceptions;
using SFA.DAS.RoATPService.Application.Handlers;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Application.Validators;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Application.UnitTests
{

    [TestFixture]
    public class UpdateOrganisationCharityNumberHandlerTests
    {
        private Mock<ILogger<UpdateOrganisationCharityNumberHandler>> _logger;
        private Mock<IOrganisationValidator> _validator;
        private Mock<IUpdateOrganisationRepository> _updateOrganisationRepository;
        private Mock<IOrganisationRepository> _repository;
        private UpdateOrganisationCharityNumberHandler _handler;
        private Mock<IAuditLogService> _auditLogService;

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<UpdateOrganisationCharityNumberHandler>>();
            _validator = new Mock<IOrganisationValidator>();
            _repository = new Mock<IOrganisationRepository>();
            _validator.Setup(x => x.IsValidCharityNumber(It.IsAny<string>())).Returns(true);
            _validator.Setup(x => x.DuplicateCharityNumberInAnotherOrganisation(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(new DuplicateCheckResponse { DuplicateFound = false, DuplicateOrganisationName = "" });
            _updateOrganisationRepository = new Mock<IUpdateOrganisationRepository>();
            _repository.Setup(x => x.GetCharityNumber(It.IsAny<Guid>())).ReturnsAsync("11111111").Verifiable();
            _updateOrganisationRepository.Setup(x => x.UpdateCharityNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();
            _updateOrganisationRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true).Verifiable();
            _auditLogService = new Mock<IAuditLogService>();
            _auditLogService.Setup(x => x.CreateAuditData(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(new AuditData { FieldChanges = new List<AuditLogEntry>() });
            _auditLogService.Setup(x => x.AuditCharityNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new AuditData { FieldChanges = new List<AuditLogEntry>() });
            _handler = new UpdateOrganisationCharityNumberHandler(_logger.Object, _validator.Object, _updateOrganisationRepository.Object, _auditLogService.Object);
        }

        [Test]
        public async Task Handler_does_not_update_database_if_charity_number_invalid()
        {
            _validator.Setup(x => x.IsValidCharityNumber(It.IsAny<string>())).Returns(false);
            var request = new UpdateOrganisationCharityNumberRequest
            {
                CharityNumber = "1111222",
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            Func<Task> result = () => _handler.Handle(request, new CancellationToken());
            await result.Should().ThrowAsync<BadRequestException>();

            _auditLogService.Verify(x => x.AuditCharityNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _updateOrganisationRepository.Verify(x => x.UpdateCharityNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [Test]
        public async Task Handler_does_not_update_database_if_charity_number_unchanged()
        {
            var request = new UpdateOrganisationCharityNumberRequest
            {
                CharityNumber = "11111111",
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            _validator.Setup(x => x.DuplicateCharityNumberInAnotherOrganisation(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(new DuplicateCheckResponse { DuplicateFound = true, DuplicateOrganisationName = "Duplicate organisation name" });
            Func<Task> result = () => _handler.Handle(request, new CancellationToken());
            await result.Should().ThrowAsync<BadRequestException>();

            _auditLogService.Verify(x => x.AuditCharityNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _updateOrganisationRepository.Verify(x => x.UpdateCharityNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [Test]
        public void Handler_does_not_write_audit_log_entry_if_save_operation_fails()
        {
            _updateOrganisationRepository.Setup(x => x.UpdateCharityNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false).Verifiable();

            var request = new UpdateOrganisationCharityNumberRequest
            {
                CharityNumber = "11112222",
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            var fieldChanges = new List<AuditLogEntry>();
            fieldChanges.Add(new AuditLogEntry { FieldChanged = AuditLogField.CharityNumber, NewValue = "1111111", PreviousValue = "22222222" });
            _auditLogService.Setup(x => x.AuditCharityNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new AuditData { FieldChanges = fieldChanges });

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeFalse();

            _auditLogService.Verify(x => x.AuditCharityNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.UpdateCharityNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [Test]
        public void Handler_writes_updated_charity_number_and_audit_log_entry_to_database()
        {
            var request = new UpdateOrganisationCharityNumberRequest
            {
                CharityNumber = "11112222",
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            var fieldChanges = new List<AuditLogEntry>();
            fieldChanges.Add(new AuditLogEntry { FieldChanged = AuditLogField.CharityNumber, NewValue = "1111111", PreviousValue = "22222222" });
            _auditLogService.Setup(x => x.AuditCharityNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new AuditData { FieldChanges = fieldChanges });

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeTrue();

            _auditLogService.Verify(x => x.AuditCharityNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.UpdateCharityNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Once);
        }
    }

}