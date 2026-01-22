using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisationsStatusEvents;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers.OrganisationsControllerTests;

public class OrganisationsControllerGetOrganisationStatusEventsTests
{
    [Test, MoqAutoData]
    public async Task GetOrganisationStatusEvents_InvokesMediator(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationsController sut,
        GetOrganisationStatusEventsQueryResult expected,
        int sinceEventId,
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken)
    {
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetOrganisationStatusEventsQuery>(), cancellationToken))
            .ReturnsAsync(expected);

        await sut.GetOrganisationStatusEvents(sinceEventId, pageSize, pageNumber, cancellationToken);

        mediatorMock.Verify(m => m.Send(
            It.Is<GetOrganisationStatusEventsQuery>(q =>
                q.SinceEventId == sinceEventId &&
                q.PageSize == pageSize &&
                q.PageNumber == pageNumber),
            cancellationToken), Times.Once);
    }

    [MoqInlineAutoData(-1)]
    [MoqInlineAutoData(1001)]
    public async Task GetOrganisationStatusEvents_DefaultsFilters_AndInvokesMediator(
        int pageSize,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationsController sut,
        GetOrganisationStatusEventsQueryResult expected,
        CancellationToken cancellationToken)
    {
        int sinceEventId = -1;
        int pageNumber = -1;

        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetOrganisationStatusEventsQuery>(), cancellationToken))
            .ReturnsAsync(expected);

        await sut.GetOrganisationStatusEvents(sinceEventId, pageSize, pageNumber, cancellationToken);

        mediatorMock.Verify(m => m.Send(
            It.Is<GetOrganisationStatusEventsQuery>(q =>
                q.SinceEventId == 0 &&
                q.PageSize == 1000 &&
                q.PageNumber == 1),
            cancellationToken), Times.Once);
    }
}
