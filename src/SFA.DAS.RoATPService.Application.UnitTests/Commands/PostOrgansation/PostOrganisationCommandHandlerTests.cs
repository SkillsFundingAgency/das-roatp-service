using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.PostOrganisation;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Domain.Common;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;
using Organisation = SFA.DAS.RoATPService.Domain.Entities.Organisation;
using ProviderType = SFA.DAS.RoATPService.Domain.Common.ProviderType;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.PostOrgansation;
public class PostOrganisationCommandHandlerTests
{
    private const int OrganisationTypeId = 3;
    private const string legalName = "<legal name";
    private const string tradingName = ">trading name";
    private const string legalNameSanitised = "legal name";
    private const string tradingNameSanitised = "trading name";


    [Test]
    [RecursiveMoqInlineAutoData(ProviderType.Main, false, OrganisationStatus.OnBoarding)]
    [RecursiveMoqInlineAutoData(ProviderType.Employer, false, OrganisationStatus.OnBoarding)]
    [RecursiveMoqInlineAutoData(ProviderType.Supporting, true, OrganisationStatus.Active)]
    public async Task Handle_CallsRepositoryWithExpectedOrganisation(
        ProviderType providerType,
        bool hasStartDate,
        OrganisationStatus expectedOrganisatonStatus,
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        [Frozen] Mock<ITextSanitiser> textSanitiserMock,
        Organisation organisation,
        string userId,
        PostOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {

        textSanitiserMock.Setup((x => x.SanitiseInputText(legalName))).Returns(legalNameSanitised);
        textSanitiserMock.Setup((x => x.SanitiseInputText(tradingName))).Returns(tradingNameSanitised);
        organisation.OrganisationTypeId = 1;
        organisation.LegalName = legalName;
        organisation.TradingName = tradingName;



        PostOrganisationCommand command = new PostOrganisationCommand
        {
            Ukprn = organisation.Ukprn,
            LegalName = organisation.LegalName,
            TradingName = organisation.TradingName,
            CompanyNumber = organisation.CompanyNumber,
            CharityNumber = organisation.CharityNumber,
            ProviderType = providerType,
            OrganisationTypeId = organisation.OrganisationTypeId,
            RequestingUserId = organisation.UpdatedBy
        };

        var actual = await sut.Handle(command, cancellationToken);

        organisationsRepositoryMock.Verify(x => x.CreateOrganisation(
            It.Is<Organisation>(o =>
                o.CreatedAt <= DateTime.UtcNow
                && o.CreatedAt >= DateTime.UtcNow.AddSeconds(-20)
                && o.CreatedBy == command.RequestingUserId
                && o.Status == expectedOrganisatonStatus
                && o.ProviderType == providerType
                && o.OrganisationTypeId == command.OrganisationTypeId
                && o.Ukprn == command.Ukprn
                && o.LegalName == legalNameSanitised
                && o.TradingName == tradingNameSanitised
                && o.StatusDate <= DateTime.UtcNow
                && o.StatusDate >= DateTime.UtcNow.AddSeconds(-20)
                && o.CompanyNumber == command.CompanyNumber
                && o.CharityNumber == command.CharityNumber
                && o.ApplicationDeterminedDate <= DateTime.UtcNow
                && o.ApplicationDeterminedDate >= DateTime.UtcNow.AddSeconds(-20)
                && hasStartDate
                    ? o.StartDate >= DateTime.UtcNow.AddSeconds(-20)
                      && o.StartDate <= DateTime.UtcNow
                    : o.StartDate == null
                && o.ApplicationDeterminedDate >= DateTime.UtcNow.AddSeconds(-20)
                && o.OrganisationData.CompanyNumber.Equals(command.CompanyNumber, StringComparison.OrdinalIgnoreCase)
                && o.OrganisationData.CharityNumber == command.CharityNumber
                && o.OrganisationData.ApplicationDeterminedDate <= DateTime.UtcNow
                && o.OrganisationData.ApplicationDeterminedDate >= DateTime.UtcNow.AddSeconds(-20)
                && hasStartDate
                        ? o.OrganisationData.StartDate >= DateTime.UtcNow.AddSeconds(-20)
                          && o.OrganisationData.StartDate <= DateTime.UtcNow
                        : o.StartDate == null
            ),
            It.IsAny<Audit>(),
            It.IsAny<OrganisationStatusEvent>(),
            cancellationToken), Times.Once);
        Assert.That(actual.Result.IsSuccess, Is.True);
    }

    [Test]
    [RecursiveMoqInlineAutoData]
    public async Task Handle_CallsRepositoryWithExpectedAudit(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
       [Frozen] Mock<ITextSanitiser> textSanitiserMock,
       Organisation organisation,
       string userId,
       PostOrganisationCommandHandler sut,
       CancellationToken cancellationToken)
    {
        organisation.OrganisationTypeId = 1;
        organisation.LegalName = legalName;
        organisation.TradingName = tradingName;

        PostOrganisationCommand command = new PostOrganisationCommand
        {
            Ukprn = organisation.Ukprn,
            LegalName = organisation.LegalName,
            TradingName = organisation.TradingName,
            CompanyNumber = organisation.CompanyNumber,
            CharityNumber = organisation.CharityNumber,
            ProviderType = ProviderType.Main,
            OrganisationTypeId = organisation.OrganisationTypeId,
            RequestingUserId = organisation.UpdatedBy
        };

        var actual = await sut.Handle(command, cancellationToken);

        organisationsRepositoryMock.Verify(x => x.CreateOrganisation(
            It.IsAny<Organisation>(),
            It.Is<Audit>(
                a => a.UpdatedBy == command.RequestingUserId
                && a.UpdatedAt <= DateTime.UtcNow
                && a.UpdatedAt >= DateTime.UtcNow.AddSeconds(-20)
                && a.OrganisationId == a.AuditData.OrganisationId
                && a.AuditData.FieldChanges.Count == 0
                && a.AuditData.UpdatedBy == command.RequestingUserId
                && a.AuditData.UpdatedAt <= DateTime.UtcNow
                && a.AuditData.UpdatedAt >= DateTime.UtcNow.AddSeconds(-20)
                && a.OrganisationId == a.AuditData.OrganisationId
            ),
            It.IsAny<OrganisationStatusEvent>(),
            cancellationToken), Times.Once);
        Assert.That(actual.Result.IsSuccess, Is.True);
    }

    [Test]
    [RecursiveMoqInlineAutoData(ProviderType.Main, OrganisationStatus.OnBoarding)]
    [RecursiveMoqInlineAutoData(ProviderType.Employer, OrganisationStatus.OnBoarding)]
    [RecursiveMoqInlineAutoData(ProviderType.Supporting, OrganisationStatus.Active)]
    public async Task Handle_CallsRepositoryWithExpectedOrganisationStatusEvent(
        ProviderType providerType,
        OrganisationStatus expectedOrganisatonStatus,
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        [Frozen] Mock<ITextSanitiser> textSanitiserMock,
        Organisation organisation,
        string userId,
        PostOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        var isSupportingProvider = providerType == ProviderType.Supporting;
        organisation.OrganisationTypeId = 1;
        organisation.LegalName = legalName;
        organisation.TradingName = tradingName;
        organisation.ProviderType = providerType;

        PostOrganisationCommand command = new PostOrganisationCommand
        {
            Ukprn = organisation.Ukprn,
            LegalName = organisation.LegalName,
            TradingName = organisation.TradingName,
            CompanyNumber = organisation.CompanyNumber,
            CharityNumber = organisation.CharityNumber,
            ProviderType = providerType,
            OrganisationTypeId = organisation.OrganisationTypeId,
            RequestingUserId = organisation.UpdatedBy
        };

        var actual = await sut.Handle(command, cancellationToken);

        organisationsRepositoryMock.Verify(x => x.CreateOrganisation(
            It.IsAny<Organisation>(),
            It.IsAny<Audit>(),
            It.Is<OrganisationStatusEvent>(
                o =>
                    o.CreatedOn <= DateTime.UtcNow
                    && o.CreatedOn >= DateTime.UtcNow.AddSeconds(-20)
                    && o.OrganisationStatus == expectedOrganisatonStatus
                    && o.Ukprn == organisation.Ukprn
                ),
            cancellationToken), Times.Once);
        Assert.That(actual.Result.IsSuccess, Is.True);
    }
}
