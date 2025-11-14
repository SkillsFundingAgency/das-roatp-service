using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisations;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers.OrganisationsControllerTests;
public class OrganisationsControllerGetOrganisationsTests
{
    [Test, MoqAutoData]
    public async Task GetOrganisations_InvokesMediator(
        [Frozen] Mock<IMediator> meditorMock,
        [Greedy] OrganisationsController sut,
        string searchTerm,
        CancellationToken cancellationToken)
    {
        await sut.GetOrganisations(searchTerm, cancellationToken);

        meditorMock.Verify(m => m.Send(It.Is<GetOrganisationsQuery>(q => q.SearchTerm == searchTerm), cancellationToken), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task GetOrganisations_ReturnsExpectedResult(
        [Frozen] Mock<IMediator> meditorMock,
        [Greedy] OrganisationsController sut,
        GetOrganisationsQueryResult expected,
        string searchTerm,
        CancellationToken cancellationToken)
    {
        meditorMock.Setup(m => m.Send(It.IsAny<GetOrganisationsQuery>(), cancellationToken)).ReturnsAsync(expected);

        IActionResult actual = await sut.GetOrganisations(searchTerm, cancellationToken);

        actual.As<OkObjectResult>().Value.Should().Be(expected);
    }
}