using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Domain;
using SFA.DAS.RoATPService.Domain.Configuration;

namespace SFA.DAS.RoATPService.Application.UnitTests;

[TestFixture]
public class AuditLogOrganisationTests
{
    private RegisterAuditLogSettings _settings;
    private Mock<IOrganisationRepository> _organisationRepository;
    private Mock<ILookupDataRepository> _lookupRepository;
    private Mock<IOrganisationStatusManager> _organisationStatusManager;

    [SetUp]
    public void Before_each_test()
    {
        _organisationRepository = new Mock<IOrganisationRepository>();
        _lookupRepository = new Mock<ILookupDataRepository>();
        _settings = new RegisterAuditLogSettings();
        _organisationStatusManager = new Mock<IOrganisationStatusManager>();
    }

    [TestCase("first name", "second name", true)]
    [TestCase("second name", "first name", true)]
    [TestCase("first name", "first name", null)]
    [TestCase("second name", "second name", null)]
    public void Audit_log_checks_legal_name_audit_is_as_expected(string currentName, string newName, bool? auditChangesMade)
    {
        var _organisationId = Guid.NewGuid();
        var username = "user name";
        _organisationRepository.Setup(x => x.GetLegalName(It.IsAny<Guid>())).ReturnsAsync(currentName);
        _organisationRepository.Setup(x => x.GetOrganisationType(It.IsAny<Guid>())).ReturnsAsync(1);
        _organisationRepository.Setup(x => x.GetOrganisationStatus(It.IsAny<Guid>())).ReturnsAsync(1);
        _organisationRepository.Setup(x => x.GetStartDate(It.IsAny<Guid>())).ReturnsAsync(DateTime.Today);
        _organisationRepository.Setup(x => x.GetApplicationDeterminedDate(It.IsAny<Guid>()))
            .ReturnsAsync(DateTime.Today);
        _lookupRepository.Setup(x => x.GetProviderTypes()).ReturnsAsync(new List<ProviderType>());
        _organisationStatusManager.Setup(x =>
                x.ShouldChangeStatustoActiveAndSetStartDateToToday(It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<int>()))
            .Returns(true);
        var auditLogService = new AuditLogService(_settings, _organisationRepository.Object, _lookupRepository.Object, _organisationStatusManager.Object);
        var auditData = auditLogService.AuditOrganisation(new UpdateOrganisationCommand { LegalName = newName, OrganisationId = _organisationId, Username = username, OrganisationTypeId = 1, ApplicationDeterminedDate = DateTime.Today });

        Assert.AreEqual(auditChangesMade, auditData?.ChangesMade);
    }

    [TestCase(1, 2, true)]
    [TestCase(2, 1, true)]
    [TestCase(1, 1, null)]
    [TestCase(2, 2, null)]
    public void Audit_log_checks_organisation_type_id_audit_is_as_expected(int previousOrgTypeId, int newOrgTypeId, bool? auditChangesMade)
    {
        var _organisationId = Guid.NewGuid();
        var username = "user name";
        _organisationRepository.Setup(x => x.GetLegalName(It.IsAny<Guid>())).ReturnsAsync("legal name");
        _organisationRepository.Setup(x => x.GetOrganisationType(It.IsAny<Guid>())).ReturnsAsync(previousOrgTypeId);
        _organisationRepository.Setup(x => x.GetOrganisationStatus(It.IsAny<Guid>())).ReturnsAsync(1);
        _organisationRepository.Setup(x => x.GetStartDate(It.IsAny<Guid>())).ReturnsAsync(DateTime.Today);
        _organisationRepository.Setup(x => x.GetApplicationDeterminedDate(It.IsAny<Guid>()))
            .ReturnsAsync(DateTime.Today);
        _lookupRepository.Setup(x => x.GetProviderTypes()).ReturnsAsync(new List<ProviderType>());
        _organisationStatusManager.Setup(x =>
                x.ShouldChangeStatustoActiveAndSetStartDateToToday(It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<int>()))
            .Returns(true);
        var auditLogService = new AuditLogService(_settings, _organisationRepository.Object, _lookupRepository.Object, _organisationStatusManager.Object);
        var auditData = auditLogService.AuditOrganisation(new UpdateOrganisationCommand { LegalName = "legal name", OrganisationId = _organisationId, Username = username, OrganisationTypeId = newOrgTypeId, ApplicationDeterminedDate = DateTime.Today });

        Assert.AreEqual(auditChangesMade, auditData?.ChangesMade);
    }
}
