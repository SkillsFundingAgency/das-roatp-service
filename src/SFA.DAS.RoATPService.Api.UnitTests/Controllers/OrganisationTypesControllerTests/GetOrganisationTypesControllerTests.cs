using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisationTypes;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers.OrganisationTypesControllerTests;
public class GetOrganisationTypesControllerTests
{
    [Test, MoqAutoData]
    public async Task GetOrganisationTypes_CallsMediator_ReturnsOkResponse(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationTypesController sut,
        GetOrganisationTypesQueryResult expectedQueryResult
    )
    {
        mediatorMock.Setup(m => m.Send(It.IsAny<GetOrganisationTypesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedQueryResult);

        var result = await sut.GetOrganisationTypes(CancellationToken.None);

        result.As<OkObjectResult>().Value.Should().Be(expectedQueryResult);
        mediatorMock.Verify(m => m.Send(It.IsAny<GetOrganisationTypesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
