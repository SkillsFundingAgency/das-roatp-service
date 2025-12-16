using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.UpsertOrganisation;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Domain.Common;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;
using Organisation = SFA.DAS.RoATPService.Domain.Entities.Organisation;
using ProviderType = SFA.DAS.RoATPService.Domain.Common.ProviderType;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.UpsertOrgansation;

public class UpsertOrganisationCommandHandlerInsertTests
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
        command.IsNewOrganisation = true;

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
        organisationsRepositoryMock.VerifyNoOtherCalls();
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
        command.IsNewOrganisation = true;

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
        command.IsNewOrganisation = true;
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
        command.IsNewOrganisation = true;
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

public class UpsertOrganisationCommandHandlerUpdateTests
{
    [Test, MoqAutoData]
    public void Handle_OrganisationBotFound_ThrowsException(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        UpsertOrganisationCommand command,
        UpsertOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        command.IsNewOrganisation = false;
        organisationsRepositoryMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, cancellationToken)).ReturnsAsync(() => null);

        Assert.ThrowsAsync<InvalidOperationException>(() => sut.Handle(command, cancellationToken));
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_IsExistingOrganisation_InvokesRepositoryToUpdateOrganisation(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        Organisation existingOrganisation,
        UpsertOrganisationCommand command,
        UpsertOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        command.IsNewOrganisation = false;
        organisationsRepositoryMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, cancellationToken)).ReturnsAsync(existingOrganisation);

        var actual = await sut.Handle(command, cancellationToken);

        organisationsRepositoryMock.Verify(x => x.GetOrganisationByUkprn(command.Ukprn, cancellationToken), Times.Once);
        organisationsRepositoryMock.Verify(x => x.UpdateOrganisation(
            existingOrganisation,
            It.IsAny<Audit>(),
            null,
            cancellationToken), Times.Once);
        organisationsRepositoryMock.VerifyNoOtherCalls();
        Assert.That(actual.Result.IsSuccess, Is.True);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_IsExistingOrganisation_UpdatesExistingOrganisationWithNewValues(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        Organisation existingOrganisation,
        UpsertOrganisationCommand command,
        UpsertOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        existingOrganisation.ApplicationDeterminedDate = DateTime.UtcNow.AddDays(-10);
        command.Ukprn = existingOrganisation.Ukprn;
        command.IsNewOrganisation = false;
        organisationsRepositoryMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, cancellationToken)).ReturnsAsync(existingOrganisation);

        await sut.Handle(command, cancellationToken);

        organisationsRepositoryMock.Verify(x => x.UpdateOrganisation(
            It.Is<Organisation>(o =>
                o.Ukprn == command.Ukprn &&
                o.ProviderType == command.ProviderType &&
                o.OrganisationTypeId == command.OrganisationTypeId &&
                o.ApplicationDeterminedDate >= DateTime.UtcNow.Date &&
                o.LegalName == command.LegalName &&
                o.TradingName == command.TradingName &&
                o.CompanyNumber == command.CompanyNumber.ToUpper() &&
                o.CharityNumber == command.CharityNumber &&
                o.UpdatedAt >= DateTime.UtcNow.AddMinutes(-1) &&
                o.UpdatedBy == command.RequestingUserId),
            It.IsAny<Audit>(),
            null,
            cancellationToken), Times.Once);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_IsExistingOrganisation_StatusIsNotUpdated(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        Organisation existingOrganisation,
        UpsertOrganisationCommand command,
        UpsertOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        existingOrganisation.ApplicationDeterminedDate = DateTime.UtcNow.AddDays(-10);
        command.Ukprn = existingOrganisation.Ukprn;
        command.IsNewOrganisation = false;
        organisationsRepositoryMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, cancellationToken)).ReturnsAsync(existingOrganisation);
        Organisation clonedExistingOrganisation = new()
        {
            Id = existingOrganisation.Id,
            Ukprn = existingOrganisation.Ukprn,
            LegalName = existingOrganisation.LegalName,
            TradingName = existingOrganisation.TradingName,
            CompanyNumber = existingOrganisation.CompanyNumber,
            CharityNumber = existingOrganisation.CharityNumber,
            ProviderType = existingOrganisation.ProviderType,
            OrganisationTypeId = existingOrganisation.OrganisationTypeId,
            CreatedAt = existingOrganisation.CreatedAt,
            CreatedBy = existingOrganisation.CreatedBy,
            Status = existingOrganisation.Status,
            StatusDate = existingOrganisation.StatusDate,
            ApplicationDeterminedDate = existingOrganisation.ApplicationDeterminedDate
        };

        await sut.Handle(command, cancellationToken);

        organisationsRepositoryMock.Verify(x => x.UpdateOrganisation(
            It.Is<Organisation>(o =>
                o.Ukprn == command.Ukprn &&
                o.Status == clonedExistingOrganisation.Status &&
                o.StatusDate == clonedExistingOrganisation.StatusDate),
            It.IsAny<Audit>(),
            null,
            cancellationToken), Times.Once);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_IsExistingOrganisation_AddsChangedFieldsToAudit(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        Organisation existingOrganisation,
        UpsertOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        existingOrganisation.ApplicationDeterminedDate = DateTime.UtcNow.AddDays(-10);
        existingOrganisation.ProviderType = ProviderType.Supporting;
        existingOrganisation.OrganisationTypeId = 1;

        Organisation clonedExistingOrganisation = new()
        {
            Id = existingOrganisation.Id,
            Ukprn = existingOrganisation.Ukprn,
            LegalName = existingOrganisation.LegalName,
            TradingName = existingOrganisation.TradingName,
            CompanyNumber = existingOrganisation.CompanyNumber,
            CharityNumber = existingOrganisation.CharityNumber,
            ProviderType = existingOrganisation.ProviderType,
            OrganisationTypeId = existingOrganisation.OrganisationTypeId,
            CreatedAt = existingOrganisation.CreatedAt,
            CreatedBy = existingOrganisation.CreatedBy,
            Status = existingOrganisation.Status,
            StatusDate = existingOrganisation.StatusDate,
            ApplicationDeterminedDate = existingOrganisation.ApplicationDeterminedDate
        };

        UpsertOrganisationCommand command = new()
        {
            Ukprn = existingOrganisation.Ukprn,
            IsNewOrganisation = false,
            ProviderType = ProviderType.Main,
            OrganisationTypeId = 2,
            LegalName = existingOrganisation.LegalName + " Changed",
            TradingName = existingOrganisation.TradingName + " Changed",
            CompanyNumber = (existingOrganisation.CompanyNumber ?? "12345678") + "9",
            CharityNumber = (existingOrganisation.CharityNumber ?? "87654321") + "0",
            RequestingUserId = "TestUser",
        };
        organisationsRepositoryMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, cancellationToken)).ReturnsAsync(existingOrganisation);

        await sut.Handle(command, cancellationToken);

        organisationsRepositoryMock.Verify(x => x.UpdateOrganisation(
            It.IsAny<Organisation>(),
            It.Is<Audit>(a =>
                a.OrganisationId == existingOrganisation.Id &&
                a.UpdatedAt >= DateTime.UtcNow.AddMinutes(-1) &&
                a.UpdatedBy == command.RequestingUserId &&
                a.AuditData.OrganisationId == existingOrganisation.Id &&
                a.AuditData.UpdatedAt >= DateTime.UtcNow.AddMinutes(-1) &&
                a.AuditData.UpdatedBy == command.RequestingUserId &&
                a.AuditData.FieldChanges.Count == 6
                && VerifyAuditFields(AuditLogField.LegalName, a.AuditData.FieldChanges, clonedExistingOrganisation.LegalName, command.LegalName)
                && VerifyAuditFields(AuditLogField.TradingName, a.AuditData.FieldChanges, clonedExistingOrganisation.TradingName, command.TradingName)
                && VerifyAuditFields(AuditLogField.CompanyNumber, a.AuditData.FieldChanges, clonedExistingOrganisation.CompanyNumber, command.CompanyNumber)
                && VerifyAuditFields(AuditLogField.CharityNumber, a.AuditData.FieldChanges, clonedExistingOrganisation.CharityNumber, command.CharityNumber)
                && VerifyAuditFields(AuditLogField.ProviderType, a.AuditData.FieldChanges, clonedExistingOrganisation.ProviderType.ToString(), command.ProviderType.ToString())
                && VerifyAuditFields(AuditLogField.OrganisationType, a.AuditData.FieldChanges, clonedExistingOrganisation.OrganisationTypeId.ToString(), command.OrganisationTypeId.ToString())),
            null,
            cancellationToken), Times.Once);
    }

    private static bool VerifyAuditFields(string fieldChanged, List<Domain.AuditLogEntry> fieldChanges, string oldValue, string newValue)
    {
        return fieldChanges.Exists(fc =>
            fc.FieldChanged == fieldChanged &&
            fc.PreviousValue == oldValue &&
            fc.NewValue == newValue);
    }
}
