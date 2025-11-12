using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.PatchOrganisation;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Domain.Common;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.PatchOrganisation;
public class PatchOrganisationCommandHandlerTests
{
    private const int RemovedReasonId = 2;
    private const int OrganisationTypeId = 3;

    [Test, MoqAutoData]
    public async Task Handle_UkprnNotFound_ReturnsFalse(
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
        organisation.ProviderType = Domain.Common.ProviderType.Main;
        organisation.OrganisationTypeId = 1;

        var patchDoc = new JsonPatchDocument<PatchOrganisationModel>();
        patchDoc.Replace(o => o.Status, OrganisationStatus.Removed);
        patchDoc.Replace(o => o.RemovedReasonId, RemovedReasonId);
        patchDoc.Replace(o => o.ProviderType, Domain.Common.ProviderType.Employer);
        patchDoc.Replace(o => o.OrganisationTypeId, OrganisationTypeId);

        PatchOrganisationCommand command = new(organisation.Ukprn, userId, patchDoc);
        organisationsRepositoryMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, cancellationToken)).ReturnsAsync(organisation);

        var actual = await sut.Handle(command, cancellationToken);

        organisationsRepositoryMock.Verify(x => x.UpdateOrganisation(
            It.Is<Organisation>(o =>
                o.Status == OrganisationStatus.Removed &&
                o.RemovedReasonId == RemovedReasonId &&
                o.ProviderType == Domain.Common.ProviderType.Employer &&
                o.OrganisationTypeId == OrganisationTypeId &&
                o.UpdatedBy == userId &&
                o.UpdatedAt.GetValueOrDefault().Date == DateTime.UtcNow.Date
            ),
            It.Is<Audit>(a => a.AuditData.FieldChanges.Count == 4),
            It.IsAny<OrganisationStatusEvent>(),
            false, It.IsAny<string>(),
            cancellationToken), Times.Once);

        Assert.That(actual.Result.IsSuccess, Is.True);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_UkprnFound_UpdatesOrganisationStatusAndCreatesStatusEvent(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        Organisation organisation,
        string userId,
        PatchOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        organisation.Status = OrganisationStatus.OnBoarding;
        organisation.RemovedReasonId = null;

        var expectedStatus = OrganisationStatus.Active;

        var patchDoc = new JsonPatchDocument<PatchOrganisationModel>();
        patchDoc.Replace(o => o.Status, expectedStatus);

        PatchOrganisationCommand command = new(organisation.Ukprn, userId, patchDoc);
        organisationsRepositoryMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, cancellationToken)).ReturnsAsync(organisation);

        var actual = await sut.Handle(command, cancellationToken);

        organisationsRepositoryMock.Verify(x => x.UpdateOrganisation(
            It.Is<Organisation>(o =>
                o.Status == expectedStatus &&
                // below are unchanged
                o.RemovedReasonId == organisation.RemovedReasonId &&
                o.ProviderType == organisation.ProviderType &&
                o.OrganisationTypeId == organisation.OrganisationTypeId
            ),
            It.Is<Audit>(a =>
                a.AuditData.FieldChanges.Count == 1
                && a.AuditData.FieldChanges[0].FieldChanged == AuditLogField.OrganisationStatus
                && a.AuditData.FieldChanges[0].PreviousValue == OrganisationStatus.OnBoarding.ToString()
                && a.AuditData.FieldChanges[0].NewValue == expectedStatus.ToString()
            ),
            It.Is<OrganisationStatusEvent>(e =>
                e.OrganisationStatus == expectedStatus
                && e.Ukprn == organisation.Ukprn
            ),
            false, It.IsAny<string>(),
            cancellationToken), Times.Once);

        Assert.That(actual.Result.IsSuccess, Is.True);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_UkprnFound_UpdatesStatusAndClearsRemovedReasonId(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        Organisation organisation,
        string userId,
        PatchOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        organisation.Status = OrganisationStatus.Removed;
        organisation.RemovedReasonId = 1;

        var expectedStatus = OrganisationStatus.Active;

        var patchDoc = new JsonPatchDocument<PatchOrganisationModel>();
        patchDoc.Replace(o => o.Status, expectedStatus);

        PatchOrganisationCommand command = new(organisation.Ukprn, userId, patchDoc);
        organisationsRepositoryMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, cancellationToken)).ReturnsAsync(organisation);

        var actual = await sut.Handle(command, cancellationToken);

        organisationsRepositoryMock.Verify(x => x.UpdateOrganisation(
            It.Is<Organisation>(o =>
                o.Status == expectedStatus &&
                o.RemovedReasonId == null &&
                // below are unchanged
                o.ProviderType == organisation.ProviderType &&
                o.OrganisationTypeId == organisation.OrganisationTypeId
            ),
            It.Is<Audit>(a =>
                a.AuditData.FieldChanges.Count == 2
                && a.AuditData.FieldChanges[0].FieldChanged == AuditLogField.OrganisationStatus
                && a.AuditData.FieldChanges[0].PreviousValue == OrganisationStatus.Removed.ToString()
                && a.AuditData.FieldChanges[0].NewValue == expectedStatus.ToString()
                && a.AuditData.FieldChanges[1].FieldChanged == AuditLogField.RemovedReason
                && a.AuditData.FieldChanges[1].PreviousValue == "1"
                && a.AuditData.FieldChanges[1].NewValue == "null"
            ),
            It.Is<OrganisationStatusEvent>(e =>
                e.OrganisationStatus == expectedStatus
                && e.Ukprn == organisation.Ukprn
            ),
            false, It.IsAny<string>(),
            cancellationToken), Times.Once);

        Assert.That(actual.Result.IsSuccess, Is.True);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_UkprnFound_NoChanges_AvoidsRepositoryCall(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        Organisation organisation,
        string userId,
        PatchOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {

        var patchDoc = new JsonPatchDocument<PatchOrganisationModel>();
        patchDoc.Replace(o => o.Status, organisation.Status);
        patchDoc.Replace(o => o.ProviderType, organisation.ProviderType);
        patchDoc.Replace(o => o.RemovedReasonId, organisation.RemovedReasonId);
        patchDoc.Replace(o => o.OrganisationTypeId, organisation.OrganisationTypeId);

        PatchOrganisationCommand command = new(organisation.Ukprn, userId, patchDoc);
        organisationsRepositoryMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, cancellationToken)).ReturnsAsync(organisation);

        var actual = await sut.Handle(command, cancellationToken);

        organisationsRepositoryMock.Verify(x => x.UpdateOrganisation(
            It.IsAny<Organisation>(),
            It.IsAny<Audit>(),
            It.IsAny<OrganisationStatusEvent>(),
            It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<CancellationToken>()),

            Times.Never);

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
            null,
            false, It.IsAny<string>(),
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
        organisation.ProviderType = Domain.Common.ProviderType.Main;
        var patchDoc = new JsonPatchDocument<PatchOrganisationModel>();
        patchDoc.Replace(o => o.ProviderType, Domain.Common.ProviderType.Employer);
        PatchOrganisationCommand command = new(organisation.Ukprn, userId, patchDoc);
        organisationsRepositoryMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, cancellationToken)).ReturnsAsync(organisation);
        var actual = await sut.Handle(command, cancellationToken);
        organisationsRepositoryMock.Verify(x => x.UpdateOrganisation(
            It.Is<Organisation>(o =>
                o.Status == organisation.Status &&
                o.RemovedReasonId == organisation.RemovedReasonId &&
                o.ProviderType == Domain.Common.ProviderType.Employer &&
                o.OrganisationTypeId == organisation.OrganisationTypeId
            ),
            It.Is<Audit>(a =>
                a.AuditData.FieldChanges.Count == 1
                && a.AuditData.FieldChanges[0].FieldChanged == AuditLogField.ProviderType
                && a.AuditData.FieldChanges[0].PreviousValue == Domain.Common.ProviderType.Main.ToString()
                && a.AuditData.FieldChanges[0].NewValue == Domain.Common.ProviderType.Employer.ToString()
            ),
            null,
            false, It.IsAny<string>(),
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
            null,
            false, It.IsAny<string>(),
            cancellationToken), Times.Once);
        Assert.That(actual.Result.IsSuccess, Is.True);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_UkprnFound_UpdatesProviderTypeToSupporting(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        Organisation organisation,
        string userId,
        PatchOrganisationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        organisation.ProviderType = Domain.Common.ProviderType.Main;
        organisation.OrganisationCourseTypes = new List<OrganisationCourseType>
        {
            new() { CourseType = new CourseType { Id = (int)LearningType.Standard, IsActive = true, LearningType = LearningType.Standard } },
            new() { CourseType = new CourseType { Id = (int)LearningType.ShortCourse, IsActive = true, LearningType = LearningType.ShortCourse } }
        };

        var patchDoc = new JsonPatchDocument<PatchOrganisationModel>();
        patchDoc.Replace(o => o.ProviderType, Domain.Common.ProviderType.Supporting);
        PatchOrganisationCommand command = new(organisation.Ukprn, userId, patchDoc);
        organisationsRepositoryMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, cancellationToken)).ReturnsAsync(organisation);
        var actual = await sut.Handle(command, cancellationToken);
        organisationsRepositoryMock.Verify(x => x.UpdateOrganisation(
            It.Is<Organisation>(o =>
                o.Status == organisation.Status &&
                o.RemovedReasonId == organisation.RemovedReasonId &&
                o.ProviderType == Domain.Common.ProviderType.Supporting &&
                o.OrganisationTypeId == organisation.OrganisationTypeId
            ),
            It.Is<Audit>(a =>
                a.AuditData.FieldChanges.Count == 1
                && a.AuditData.FieldChanges[0].FieldChanged == AuditLogField.ProviderType
                && a.AuditData.FieldChanges[0].PreviousValue == Domain.Common.ProviderType.Main.ToString()
                && a.AuditData.FieldChanges[0].NewValue == Domain.Common.ProviderType.Supporting.ToString()
            ),
            null,
            true, userId,
            cancellationToken), Times.Once);
        Assert.That(actual.Result.IsSuccess, Is.True);
    }
}
