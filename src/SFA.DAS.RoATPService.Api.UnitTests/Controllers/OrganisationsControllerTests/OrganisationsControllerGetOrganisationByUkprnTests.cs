using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisation;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers.OrganisationsControllerTests;
public class OrganisationsControllerGetOrganisationByUkprnTests
{
    [Test, MoqAutoData]
    public async Task GetOrganisationByUkprn_InvokesMediator(
        [Frozen] Mock<IMediator> meditorMock,
        [Greedy] OrganisationsController sut,
        int ukprn,
        CancellationToken cancellationToken)
    {
        await sut.GetOrganisationByUkprn(ukprn, cancellationToken);

        meditorMock.Verify(m => m.Send(It.Is<GetOrganisationQuery>(q => q.Ukprn == ukprn), cancellationToken), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task GetOrganisationByUkprn_NullResult_ReturnsNotFound(
    [Frozen] Mock<IMediator> meditorMock,
    [Greedy] OrganisationsController sut,
    int ukprn,
    CancellationToken cancellationToken)
    {
        meditorMock.Setup(m => m.Send(It.Is<GetOrganisationQuery>(q => q.Ukprn == ukprn), cancellationToken)).ReturnsAsync(() => null);

        IActionResult actual = await sut.GetOrganisationByUkprn(ukprn, cancellationToken);

        actual.As<NotFoundResult>().Should().NotBeNull();
    }

    [Test, MoqAutoData]
    public async Task GetOrganisationByUkprn_NullResult_ReturnsNotFound(
    [Frozen] Mock<IMediator> meditorMock,
    [Greedy] OrganisationsController sut,
    int ukprn,
    GetOrganisationQueryResult expected,
    CancellationToken cancellationToken)
    {
        meditorMock.Setup(m => m.Send(It.Is<GetOrganisationQuery>(q => q.Ukprn == ukprn), cancellationToken)).ReturnsAsync(expected);

        IActionResult actual = await sut.GetOrganisationByUkprn(ukprn, cancellationToken);

        actual.As<OkObjectResult>().Value.Should().Be(expected);
    }
}
