﻿using System;
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
using SFA.DAS.RoATPService.Application.Validators;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Application.UnitTests
{

        [TestFixture]
        public class UpdateOrganisationUkprnHandlerTests
        {
            private Mock<ILogger<UpdateOrganisationUkprnHandler>> _logger;
            private Mock<IOrganisationValidator> _validator;
            private Mock<IUpdateOrganisationRepository> _repository;
            private Mock<IAuditLogRepository> _auditRepository;
            private UpdateOrganisationUkprnHandler _handler;

            [SetUp]
            public void Before_each_test()
            {
                _logger = new Mock<ILogger<UpdateOrganisationUkprnHandler>>();
                _validator = new Mock<IOrganisationValidator>();
                _validator.Setup(x => x.IsValidUKPRN(It.IsAny<long>())).Returns(true);
                _validator.Setup(x => x.DuplicateUkprnInAnotherOrganisation(11111111, It.IsAny<Guid>()))
                    .Returns(new DuplicateCheckResponse
                    {
                        DuplicateFound = true,
                        DuplicateOrganisationName = "other org"
                    });
                _validator.Setup(x => x.DuplicateUkprnInAnotherOrganisation(It.IsAny<long>(), It.IsAny<Guid>()))
                    .Returns(new DuplicateCheckResponse
                    {
                        DuplicateFound = false,
                        DuplicateOrganisationName = ""
                    });
            _repository = new Mock<IUpdateOrganisationRepository>();
                _repository.Setup(x => x.GetUkprn(It.IsAny<Guid>())).ReturnsAsync(11111111).Verifiable();
                _repository.Setup(x => x.UpdateUkprn(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();

                _auditRepository = new Mock<IAuditLogRepository>();
                _auditRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true).Verifiable();

                _handler = new UpdateOrganisationUkprnHandler(_logger.Object, _validator.Object, _repository.Object, _auditRepository.Object);
            }

            [Test]
            public void Handler_does_not_update_database_if_ukprn_invalid()
            {
                _validator.Setup(x => x.IsValidUKPRN(It.IsAny<long>())).Returns(false);
                var request = new UpdateOrganisationUkprnRequest
                {
                    Ukprn =1111222,
                    OrganisationId = Guid.NewGuid(),
                    UpdatedBy = "unit test"
                };

                Func<Task> result = async () => await
                    _handler.Handle(request, new CancellationToken());
                result.Should().Throw<BadRequestException>();

                _repository.Verify(x => x.GetUkprn(It.IsAny<Guid>()), Times.Never);
                _repository.Verify(x => x.UpdateUkprn(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<string>()), Times.Never);
                _auditRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
            }

            [Test]
            public void Handler_does_not_update_database_if_ukprn_unchanged()
            {
                var request = new UpdateOrganisationUkprnRequest
                {
                    Ukprn = 11111111,
                    OrganisationId = Guid.NewGuid(),
                    UpdatedBy = "unit test"
                };

                var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
                result.Should().BeFalse();

                _repository.Verify(x => x.GetUkprn(It.IsAny<Guid>()), Times.Once);
                _repository.Verify(x => x.UpdateUkprn(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<string>()), Times.Never);
                _auditRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
            }

            [Test]
            public void Handler_does_not_write_audit_log_entry_if_save_operation_fails()
            {
                _repository.Setup(x => x.UpdateUkprn(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(false).Verifiable();

                var request = new UpdateOrganisationUkprnRequest
                {
                    Ukprn = 11112222,
                    OrganisationId = Guid.NewGuid(),
                    UpdatedBy = "unit test"
                };

                var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
                result.Should().BeFalse();

                _repository.Verify(x => x.GetUkprn(It.IsAny<Guid>()), Times.Once);
                _repository.Verify(x => x.UpdateUkprn(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<string>()), Times.Once);
                _auditRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
            }

            [Test]
            public void Handler_writes_updated_legal_name_and_audit_log_entry_to_database()
            {
                var request = new UpdateOrganisationUkprnRequest
                {
                    Ukprn = 11112222,
                    OrganisationId = Guid.NewGuid(),
                    UpdatedBy = "unit test"
                };

                var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
                result.Should().BeTrue();

                _repository.Verify(x => x.GetUkprn(It.IsAny<Guid>()), Times.Once);
                _repository.Verify(x => x.UpdateUkprn(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<string>()), Times.Once);
                _auditRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Once);
            }
        }
    
}