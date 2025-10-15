using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisation;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetOrganisation;
public class GetOrganisationQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_OrganisationNotFound_ReturnsNull(
        [Frozen] Mock<IOrganisationsRepository> organisationRepositoryMock,
        [Frozen] Mock<IOrganisationStatusEventsRepository> organisationStatusEventRepositoryMock,
        GetOrganisationQueryHandler sut,
        GetOrganisationQuery query,
        CancellationToken cancellationToken)
    {
        //Arrange
        organisationRepositoryMock.Setup(x => x.GetOrganisationByUkprn(query.Ukprn, cancellationToken)).ReturnsAsync(() => null);
        //Act
        GetOrganisationQueryResult actual = await sut.Handle(query, cancellationToken);
        //Assert
        actual.Should().BeNull();
        organisationStatusEventRepositoryMock.Verify(o => o.GetLatestStatusChangeEvent(It.IsAny<int>(), It.IsAny<OrganisationStatus>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_OrganisationFound_ReturnsOrganisationDetails(
        [Frozen] Mock<IOrganisationsRepository> organisationRepositoryMock,
        [Frozen] Mock<IOrganisationStatusEventsRepository> organisationStatusEventRepositoryMock,
        GetOrganisationQueryHandler sut,
        GetOrganisationQuery query,
        Organisation expectedOrganisation,
        OrganisationStatusEvent expectedOrganisationStatusEvent,
        CancellationToken cancellationToken)
    {
        //Arrange
        organisationRepositoryMock.Setup(x => x.GetOrganisationByUkprn(query.Ukprn, cancellationToken)).ReturnsAsync(expectedOrganisation);
        //Act
        GetOrganisationQueryResult actual = await sut.Handle(query, cancellationToken);
        //Assert
        actual.Should().BeEquivalentTo(
            expectedOrganisation,
            o => o.ExcludingMissingMembers()
            .Excluding(o => o.OrganisationCourseTypes)
            .Excluding(o => o.OrganisationTypeId)
            .Excluding(x => x.OrganisationType));
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_OrganisationIsActive_RemovedDateIsNull(
        [Frozen] Mock<IOrganisationsRepository> organisationRepositoryMock,
        GetOrganisationQueryHandler sut,
        GetOrganisationQuery query,
        Organisation expectedOrganisation,
        OrganisationStatusEvent expectedOrganisationStatusEvent,
        CancellationToken cancellationToken)
    {
        //Arrange
        expectedOrganisation.Status = OrganisationStatus.Active;
        organisationRepositoryMock.Setup(x => x.GetOrganisationByUkprn(query.Ukprn, cancellationToken)).ReturnsAsync(expectedOrganisation);
        //Act
        GetOrganisationQueryResult actual = await sut.Handle(query, cancellationToken);
        //Assert
        actual.RemovedDate.Should().BeNull();
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_OrganisationStatusIsRemoved_GetsRemovedDateFromStatusEvent(
        [Frozen] Mock<IOrganisationsRepository> organisationRepositoryMock,
        [Frozen] Mock<IOrganisationStatusEventsRepository> organisationStatusEventRepositoryMock,
        GetOrganisationQueryHandler sut,
        GetOrganisationQuery query,
        Organisation expectedOrganisation,
        OrganisationStatusEvent expectedOrganisationStatusEvent,
        CancellationToken cancellationToken)
    {
        //Arrange
        expectedOrganisation.Status = OrganisationStatus.Removed;
        organisationRepositoryMock.Setup(x => x.GetOrganisationByUkprn(query.Ukprn, cancellationToken)).ReturnsAsync(expectedOrganisation);
        organisationStatusEventRepositoryMock.Setup(x => x.GetLatestStatusChangeEvent(query.Ukprn, expectedOrganisation.Status, cancellationToken)).ReturnsAsync(() => expectedOrganisationStatusEvent);
        //Act
        GetOrganisationQueryResult actual = await sut.Handle(query, cancellationToken);
        //Assert
        actual.Should().BeEquivalentTo(
            expectedOrganisation,
            o => o.ExcludingMissingMembers()
            .Excluding(o => o.OrganisationCourseTypes)
            .Excluding(o => o.OrganisationTypeId)
            .Excluding(x => x.OrganisationType));
        actual.RemovedDate.Should().Be(expectedOrganisationStatusEvent.CreatedOn);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_OrganisationStatusIsRemoved_ReturnsNoRemovedDateIfNoStatusEvent(
        [Frozen] Mock<IOrganisationsRepository> organisationRepositoryMock,
        [Frozen] Mock<IOrganisationStatusEventsRepository> organisationStatusEventRepositoryMock,
        GetOrganisationQueryHandler sut,
        GetOrganisationQuery query,
        Organisation expectedOrganisation,
        CancellationToken cancellationToken)
    {
        //Arrange
        expectedOrganisation.Status = OrganisationStatus.Removed;
        organisationRepositoryMock.Setup(x => x.GetOrganisationByUkprn(query.Ukprn, cancellationToken)).ReturnsAsync(expectedOrganisation);
        organisationStatusEventRepositoryMock.Setup(x => x.GetLatestStatusChangeEvent(query.Ukprn, expectedOrganisation.Status, cancellationToken)).ReturnsAsync(() => (OrganisationStatusEvent)null);
        //Act
        GetOrganisationQueryResult actual = await sut.Handle(query, cancellationToken);
        //Assert
        actual.Should().BeEquivalentTo(
            expectedOrganisation,
            o => o.ExcludingMissingMembers()
                .Excluding(o => o.OrganisationCourseTypes)
                .Excluding(o => o.OrganisationTypeId)
                .Excluding(x => x.OrganisationType));
        actual.RemovedDate.Should().BeNull();
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_OrganisationFound_NoAuditRecords_ReturnsLastUpdateDateFromOrganisation(
        [Frozen] Mock<IOrganisationsRepository> organisationRepositoryMock,
        [Frozen] Mock<IAuditsRepository> auditRepositoryMock,
        GetOrganisationQueryHandler sut,
        GetOrganisationQuery query,
        Organisation expectedOrganisation,
        OrganisationStatusEvent expectedOrganisationStatusEvent,
        CancellationToken cancellationToken)
    {
        //Arrange
        organisationRepositoryMock.Setup(x => x.GetOrganisationByUkprn(query.Ukprn, cancellationToken)).ReturnsAsync(expectedOrganisation);
        auditRepositoryMock.Setup(x => x.GetLastUpdatedDateForOrganisation(expectedOrganisation.Id, cancellationToken)).ReturnsAsync(() => null);
        //Act
        GetOrganisationQueryResult actual = await sut.Handle(query, cancellationToken);
        //Assert
        actual.LastUpdatedDate.Should().Be(expectedOrganisation.UpdatedAt);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_OrganisationFound_NoAuditRecords_ReturnsLastUpdateDateFromOrganisation1(
    [Frozen] Mock<IOrganisationsRepository> organisationRepositoryMock,
    [Frozen] Mock<IAuditsRepository> auditRepositoryMock,
    GetOrganisationQueryHandler sut,
    GetOrganisationQuery query,
    Organisation expectedOrganisation,
    OrganisationStatusEvent expectedOrganisationStatusEvent,
    DateTime expected,
    CancellationToken cancellationToken)
    {
        //Arrange
        organisationRepositoryMock.Setup(x => x.GetOrganisationByUkprn(query.Ukprn, cancellationToken)).ReturnsAsync(expectedOrganisation);
        auditRepositoryMock.Setup(x => x.GetLastUpdatedDateForOrganisation(expectedOrganisation.Id, cancellationToken)).ReturnsAsync(() => expected);
        //Act
        GetOrganisationQueryResult actual = await sut.Handle(query, cancellationToken);
        //Assert
        actual.LastUpdatedDate.Should().Be(expected);
    }
}
