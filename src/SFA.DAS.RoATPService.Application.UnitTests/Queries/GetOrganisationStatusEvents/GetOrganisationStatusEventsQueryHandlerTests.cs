using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisationsStatusEvents;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;
using Entities = SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetOrganisationStatusEvents;

public class GetOrganisationStatusEventsQueryHandlerTests
{
    [Test, RecursiveMoqAutoData]
    public async Task Handler_InvokesRepository(
        [Frozen] Mock<IOrganisationStatusEventsRepository> repoMock,
        GetOrganisationStatusEventsQueryHandler sut,
        GetOrganisationStatusEventsQuery query,
        CancellationToken cancellationToken)
    {
        await sut.Handle(query, cancellationToken);

        repoMock.Verify(r => r.GetOrganisationStatusEvents(query.SinceEventId, query.PageSize, query.PageNumber, cancellationToken), Times.Once);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handler_ConvertsEntityToResult(
        [Frozen] Mock<IOrganisationStatusEventsRepository> repoMock,
        List<Entities.OrganisationStatusEvent> events,
        GetOrganisationStatusEventsQueryHandler sut,
        GetOrganisationStatusEventsQuery query,
        CancellationToken cancellationToken)
    {
        repoMock.Setup(r => r.GetOrganisationStatusEvents(query.SinceEventId, query.PageSize, query.PageNumber, cancellationToken)).ReturnsAsync(events);

        GetOrganisationStatusEventsQueryResult expected = new(events.ConvertAll(e => new OrganisationStatusEvent(e.Id, e.Ukprn, e.OrganisationStatus, e.CreatedOn)));

        GetOrganisationStatusEventsQueryResult actual = await sut.Handle(query, cancellationToken);
        Assert.That(actual.Events, Is.EquivalentTo(expected.Events));
    }
}
