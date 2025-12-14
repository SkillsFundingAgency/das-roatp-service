using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.UpsertOrganisation;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Domain.Common;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;
using Organisation = SFA.DAS.RoATPService.Domain.Entities.Organisation;
using ProviderType = SFA.DAS.RoATPService.Domain.Common.ProviderType;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.UpsertOrgansation;

public class UpsertOrganisationCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_SupportingProvider_InvokesRepositoryToCreateOrganisation(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        UpsertOrganisationCommand command,
        string userId,
        UpsertOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        command.ProviderType = ProviderType.Supporting;

        var actual = await sut.Handle(command, cancellationToken);

        organisationsRepositoryMock.Verify(x => x.CreateOrganisation(
            It.Is<Organisation>(o =>
                o.CreatedAt >= DateTime.UtcNow.AddSeconds(-20)
                && o.CreatedBy == command.RequestingUserId
                && o.Status == OrganisationStatus.Active
                && o.ProviderType == ProviderType.Supporting
                && o.OrganisationTypeId == command.OrganisationTypeId
                && o.Ukprn == command.Ukprn
                && o.StatusDate >= DateTime.UtcNow.AddSeconds(-20)
                && o.CompanyNumber == command.CompanyNumber
                && o.CharityNumber == command.CharityNumber
                && o.ApplicationDeterminedDate >= DateTime.UtcNow.AddSeconds(-20)
                && o.StartDate >= DateTime.UtcNow.AddSeconds(-20)
            ),
            It.IsAny<Audit>(),
            It.IsAny<OrganisationStatusEvent>(),
            cancellationToken), Times.Once);
        Assert.That(actual.Result.IsSuccess, Is.True);
    }

    [Test]
    [RecursiveMoqInlineAutoData(ProviderType.Main)]
    [RecursiveMoqInlineAutoData(ProviderType.Employer)]
    public async Task Handle_MainOrEmployerProvider_InvokesRepositoryToCreatesOrganisation(
        ProviderType providerType,
        OrganisationStatus expectedOrganisatonStatus,
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        UpsertOrganisationCommand command,
        string userId,
        UpsertOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        command.ProviderType = providerType;

        var actual = await sut.Handle(command, cancellationToken);

        organisationsRepositoryMock.Verify(x => x.CreateOrganisation(
            It.Is<Organisation>(o =>
                o.CreatedAt >= DateTime.UtcNow.AddSeconds(-20)
                && o.CreatedBy == command.RequestingUserId
                && o.Status == OrganisationStatus.OnBoarding
                && o.ProviderType == providerType
                && o.OrganisationTypeId == command.OrganisationTypeId
                && o.Ukprn == command.Ukprn
                && o.LegalName == command.LegalName
                && o.TradingName == command.TradingName
                && o.StatusDate >= DateTime.UtcNow.AddSeconds(-20)
                && o.CompanyNumber == command.CompanyNumber
                && o.CharityNumber == command.CharityNumber
                && o.ApplicationDeterminedDate >= DateTime.UtcNow.AddSeconds(-20)
                && o.StartDate == null
            ),
            It.IsAny<Audit>(),
            It.IsAny<OrganisationStatusEvent>(),
            cancellationToken), Times.Once);
        Assert.That(actual.Result.IsSuccess, Is.True);
    }

    [Test]
    [RecursiveMoqAutoData]
    public async Task Handle_CallsRepositoryWithExpectedAudit(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        [Frozen] Mock<ITextSanitiser> textSanitiserMock,
        UpsertOrganisationCommand command,
        string userId,
        UpsertOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {
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
        UpsertOrganisationCommand command,
        string userId,
        UpsertOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        command.ProviderType = providerType;
        var actual = await sut.Handle(command, cancellationToken);

        organisationsRepositoryMock.Verify(x => x.CreateOrganisation(
            It.IsAny<Organisation>(),
            It.IsAny<Audit>(),
            It.Is<OrganisationStatusEvent>(
                o =>
                    o.CreatedOn >= DateTime.UtcNow.AddSeconds(-20)
                    && o.OrganisationStatus == expectedOrganisatonStatus
                    && o.Ukprn == command.Ukprn
                ),
            cancellationToken), Times.Once);
        Assert.That(actual.Result.IsSuccess, Is.True);
    }
}
