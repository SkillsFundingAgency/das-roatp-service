using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisationsStatusEvents;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisationStatusHistory;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers;

public class OrganisationStatusEventsControllerTests
{
    [Test, MoqAutoData]
    public async Task GetStatusEventsForUkprn_ReturnsOkResult(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationStatusEventsController sut,
        int ukprn,
        GetOrganisationStatusHistoryQueryResult queryResult,
        CancellationToken cancellationToken)
    {
        mediatorMock
            .Setup(m => m.Send(It.Is<GetOrganisationStatusHistoryQuery>(q => q.Ukprn == ukprn), cancellationToken))
            .ReturnsAsync(queryResult);

        var result = await sut.GetStatusEventsForUkprn(ukprn, cancellationToken);

        Assert.That(result, Is.TypeOf<OkObjectResult>());
        result.As<OkObjectResult>().Value.Should().Be(queryResult);
    }

    [Test, MoqAutoData]
    public async Task GetAllStatusEvents_InvokesMediator(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationStatusEventsController sut,
        GetOrganisationStatusEventsQueryResult expected,
        int sinceEventId,
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken)
    {
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetOrganisationStatusEventsQuery>(), cancellationToken))
            .ReturnsAsync(expected);

        await sut.GetAllStatusEvents(sinceEventId, pageSize, pageNumber, cancellationToken);

        mediatorMock.Verify(m => m.Send(
            It.Is<GetOrganisationStatusEventsQuery>(q =>
                q.SinceEventId == sinceEventId &&
                q.PageSize == pageSize &&
                q.PageNumber == pageNumber),
            cancellationToken), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task GetAllStatusEvents_InvokesMediator_ReturnsList(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationStatusEventsController sut,
        GetOrganisationStatusEventsQueryResult expected,
        int sinceEventId,
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken)
    {
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetOrganisationStatusEventsQuery>(), cancellationToken))
            .ReturnsAsync(expected);

        IActionResult result = await sut.GetAllStatusEvents(sinceEventId, pageSize, pageNumber, cancellationToken);

        result.As<OkObjectResult>().Value.Should().BeEquivalentTo(expected.Events);
    }

    [MoqInlineAutoData(-1)]
    [MoqInlineAutoData(1001)]
    public async Task GetAllStatusEvents_DefaultsFilters_AndInvokesMediator(
        int pageSize,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationStatusEventsController sut,
        GetOrganisationStatusEventsQueryResult expected,
        CancellationToken cancellationToken)
    {
        int sinceEventId = -1;
        int pageNumber = -1;

        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetOrganisationStatusEventsQuery>(), cancellationToken))
            .ReturnsAsync(expected);

        await sut.GetAllStatusEvents(sinceEventId, pageSize, pageNumber, cancellationToken);

        mediatorMock.Verify(m => m.Send(
            It.Is<GetOrganisationStatusEventsQuery>(q =>
                q.SinceEventId == 0 &&
                q.PageSize == 1000 &&
                q.PageNumber == 1),
            cancellationToken), Times.Once);
    }
}
