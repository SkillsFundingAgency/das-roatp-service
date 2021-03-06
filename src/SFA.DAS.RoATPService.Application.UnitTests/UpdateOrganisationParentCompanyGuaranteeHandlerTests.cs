﻿using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using Handlers;
    using Domain;
    using System;
    using System.Threading;
    using Api.Types.Models;
    using FluentAssertions;

    [TestFixture]
    public class UpdateOrganisationParentCompanyGuaranteeHandlerTests
    {
        private Mock<ILogger<UpdateOrganisationParentCompanyGuaranteeHandler>> _logger;
        private Mock<IUpdateOrganisationRepository> _updateRepository;
        private Mock<IOrganisationRepository> _repository;
        private UpdateOrganisationParentCompanyGuaranteeHandler _handler;
        private Mock<IAuditLogService> _auditLogService;

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<UpdateOrganisationParentCompanyGuaranteeHandler>>();
            _updateRepository = new Mock<IUpdateOrganisationRepository>();
            _repository = new Mock<IOrganisationRepository>();
            _repository.Setup(x => x.GetParentCompanyGuarantee(It.IsAny<Guid>())).ReturnsAsync(true).Verifiable();
            _updateRepository.Setup(x => x.UpdateParentCompanyGuarantee(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();
            _updateRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true).Verifiable();
            _auditLogService = new Mock<IAuditLogService>();
            _auditLogService
                .Setup(x => x.AuditParentCompanyGuarantee(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AuditData { FieldChanges = new List<AuditLogEntry>() });
            _handler = new UpdateOrganisationParentCompanyGuaranteeHandler(_logger.Object,  _updateRepository.Object, _auditLogService.Object);
        }

        [Test]
        public void Handler_does_not_update_database_if_parent_company_guarantee_unchanged()
        {
            var request = new UpdateOrganisationParentCompanyGuaranteeRequest
            {
                ParentCompanyGuarantee = true,
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeFalse();

            _auditLogService.Verify(x => x.AuditParentCompanyGuarantee(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
            _updateRepository.Verify(x => x.UpdateParentCompanyGuarantee(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
            _updateRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [Test]
        public void Handler_does_not_write_audit_log_entry_if_save_operation_fails()
        {
            _updateRepository.Setup(x => x.UpdateParentCompanyGuarantee(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(false).Verifiable();

            var request = new UpdateOrganisationParentCompanyGuaranteeRequest
            {
                ParentCompanyGuarantee = true,
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeFalse();

            _auditLogService.Verify(x => x.AuditParentCompanyGuarantee(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
            _updateRepository.Verify(x => x.UpdateParentCompanyGuarantee(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
            _updateRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [Test]
        public void Handler_writes_updated_parent_company_guarantee_and_audit_log_entry_to_database()
        {
            var fieldChanges = new List<AuditLogEntry>();
            fieldChanges.Add(new AuditLogEntry { FieldChanged = "Parent Company Guarantee", NewValue = "True", PreviousValue = "False" });
            _auditLogService.Setup(x => x.AuditParentCompanyGuarantee(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AuditData { FieldChanges = fieldChanges });
            var request = new UpdateOrganisationParentCompanyGuaranteeRequest
            {
                ParentCompanyGuarantee = false,
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeTrue();

            _auditLogService.Verify(x => x.AuditParentCompanyGuarantee(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
            _updateRepository.Verify(x => x.UpdateParentCompanyGuarantee(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
            _updateRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Once);
        }
    }
}
