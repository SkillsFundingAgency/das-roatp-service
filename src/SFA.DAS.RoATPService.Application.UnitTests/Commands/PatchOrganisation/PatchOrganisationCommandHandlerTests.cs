using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.PatchOrganisation;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.PatchOrganisation;
public class PatchOrganisationCommandHandlerTests
{
    private const int RemovedReasonId = 2;
    private const int OrganisationTypeId = 3;

    [Test, MoqAutoData]
    public async Task Handle_UkprnNotFound_ReturnsFalseSuccessModel(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        PatchOrganisationCommand command,
        PatchOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        organisationsRepositoryMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, cancellationToken)).ReturnsAsync(() => null);

        var actual = await sut.Handle(command, cancellationToken);

        Assert.That(actual.Result.IsSuccess, Is.False);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_UkprnFound_PatchesOrganisation(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        Organisation organisation,
        string userId,
        PatchOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        organisation.RemovedReason = null;
        organisation.Status = OrganisationStatus.Active;
        organisation.ProviderType = ProviderType.Main;
        organisation.OrganisationTypeId = 1;

        var patchDoc = new JsonPatchDocument<PatchOrganisationModel>();
        patchDoc.Replace(o => o.Status, OrganisationStatus.Removed);
        patchDoc.Replace(o => o.RemovedReasonId, RemovedReasonId);
        patchDoc.Replace(o => o.ProviderType, ProviderType.Supporting);
        patchDoc.Replace(o => o.OrganisationTypeId, OrganisationTypeId);

        PatchOrganisationCommand command = new(organisation.Ukprn, userId, patchDoc);
        organisationsRepositoryMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, cancellationToken)).ReturnsAsync(organisation);

        var actual = await sut.Handle(command, cancellationToken);

        organisationsRepositoryMock.Verify(x => x.UpdateOrganisation(
            It.Is<Organisation>(o =>
                o.Status == OrganisationStatus.Removed &&
                o.RemovedReasonId == RemovedReasonId &&
                o.ProviderType == ProviderType.Supporting &&
                o.OrganisationTypeId == OrganisationTypeId &&
                o.UpdatedBy == userId &&
                o.UpdatedAt.GetValueOrDefault().Date == DateTime.UtcNow.Date
            ),
            It.Is<Audit>(a => a.AuditData.FieldChanges.Count == 4),
            cancellationToken), Times.Once);

        Assert.That(actual.Result.IsSuccess, Is.True);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_UkprnFound_UpdatesOrganisationStatus(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        Organisation organisation,
        string userId,
        PatchOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        organisation.Status = OrganisationStatus.OnBoarding;

        var patchDoc = new JsonPatchDocument<PatchOrganisationModel>();
        patchDoc.Replace(o => o.Status, OrganisationStatus.Removed);

        PatchOrganisationCommand command = new(organisation.Ukprn, userId, patchDoc);
        organisationsRepositoryMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, cancellationToken)).ReturnsAsync(organisation);

        var actual = await sut.Handle(command, cancellationToken);

        organisationsRepositoryMock.Verify(x => x.UpdateOrganisation(
            It.Is<Organisation>(o =>
                o.Status == OrganisationStatus.Removed &&
                o.RemovedReasonId == organisation.RemovedReasonId &&
                o.ProviderType == organisation.ProviderType &&
                o.OrganisationTypeId == organisation.OrganisationTypeId
            ),
            It.Is<Audit>(a =>
                a.AuditData.FieldChanges.Count == 1
                && a.AuditData.FieldChanges[0].FieldChanged == AuditLogField.OrganisationStatus
                && a.AuditData.FieldChanges[0].PreviousValue == OrganisationStatus.OnBoarding.ToString()
                && a.AuditData.FieldChanges[0].NewValue == OrganisationStatus.Removed.ToString()
            ),
            cancellationToken), Times.Once);

        Assert.That(actual.Result.IsSuccess, Is.True);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_UkprnFound_UpdatesOrganisationType(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        Organisation organisation,
        string userId,
        PatchOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        organisation.OrganisationTypeId = 1;
        var patchDoc = new JsonPatchDocument<PatchOrganisationModel>();
        patchDoc.Replace(o => o.OrganisationTypeId, OrganisationTypeId);
        PatchOrganisationCommand command = new(organisation.Ukprn, userId, patchDoc);
        organisationsRepositoryMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, cancellationToken)).ReturnsAsync(organisation);
        var actual = await sut.Handle(command, cancellationToken);
        organisationsRepositoryMock.Verify(x => x.UpdateOrganisation(
            It.Is<Organisation>(o =>
                o.Status == organisation.Status &&
                o.RemovedReasonId == organisation.RemovedReasonId &&
                o.ProviderType == organisation.ProviderType &&
                o.OrganisationTypeId == OrganisationTypeId
            ),
            It.Is<Audit>(a =>
                a.AuditData.FieldChanges.Count == 1
                && a.AuditData.FieldChanges[0].FieldChanged == AuditLogField.OrganisationType
                && a.AuditData.FieldChanges[0].PreviousValue == "1"
                && a.AuditData.FieldChanges[0].NewValue == OrganisationTypeId.ToString()
            ),
            cancellationToken), Times.Once);
        Assert.That(actual.Result.IsSuccess, Is.True);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_UkprnFound_UpdatesProviderType(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        Organisation organisation,
        string userId,
        PatchOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        organisation.ProviderType = ProviderType.Main;
        var patchDoc = new JsonPatchDocument<PatchOrganisationModel>();
        patchDoc.Replace(o => o.ProviderType, ProviderType.Supporting);
        PatchOrganisationCommand command = new(organisation.Ukprn, userId, patchDoc);
        organisationsRepositoryMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, cancellationToken)).ReturnsAsync(organisation);
        var actual = await sut.Handle(command, cancellationToken);
        organisationsRepositoryMock.Verify(x => x.UpdateOrganisation(
            It.Is<Organisation>(o =>
                o.Status == organisation.Status &&
                o.RemovedReasonId == organisation.RemovedReasonId &&
                o.ProviderType == ProviderType.Supporting &&
                o.OrganisationTypeId == organisation.OrganisationTypeId
            ),
            It.Is<Audit>(a =>
                a.AuditData.FieldChanges.Count == 1
                && a.AuditData.FieldChanges[0].FieldChanged == AuditLogField.ProviderType
                && a.AuditData.FieldChanges[0].PreviousValue == ProviderType.Main.ToString()
                && a.AuditData.FieldChanges[0].NewValue == ProviderType.Supporting.ToString()
            ),
            cancellationToken), Times.Once);
        Assert.That(actual.Result.IsSuccess, Is.True);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_UkprnFound_RemovedReasonId(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        Organisation organisation,
        string userId,
        PatchOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        organisation.RemovedReasonId = null;
        var patchDoc = new JsonPatchDocument<PatchOrganisationModel>();
        patchDoc.Replace(o => o.RemovedReasonId, RemovedReasonId);
        PatchOrganisationCommand command = new(organisation.Ukprn, userId, patchDoc);
        organisationsRepositoryMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, cancellationToken)).ReturnsAsync(organisation);
        var actual = await sut.Handle(command, cancellationToken);
        organisationsRepositoryMock.Verify(x => x.UpdateOrganisation(
            It.Is<Organisation>(o =>
                o.Status == organisation.Status &&
                o.RemovedReasonId == RemovedReasonId &&
                o.ProviderType == organisation.ProviderType &&
                o.OrganisationTypeId == organisation.OrganisationTypeId
            ),
            It.Is<Audit>(a =>
                a.AuditData.FieldChanges.Count == 1
                && a.AuditData.FieldChanges[0].FieldChanged == AuditLogField.RemovedReason
                && a.AuditData.FieldChanges[0].PreviousValue == "null"
                && a.AuditData.FieldChanges[0].NewValue == RemovedReasonId.ToString()
            ),
            cancellationToken), Times.Once);
        Assert.That(actual.Result.IsSuccess, Is.True);
    }
}
