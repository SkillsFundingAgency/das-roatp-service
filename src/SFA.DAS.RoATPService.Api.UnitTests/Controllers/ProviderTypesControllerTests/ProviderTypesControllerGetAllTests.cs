using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.Queries.GetAllProviderTypes;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers.ProviderTypesControllerTests;

public class ProviderTypesControllerGetAllTests
{
    [Test, MoqAutoData]
    public async Task GetAll_InvokesMediator_ReturnsOkResult(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderTypesController sut,
        GetAllProviderTypesQueryResult expected)
    {
        // Arrange
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderTypesQuery>(), default))
            .ReturnsAsync(expected);
        // Act
        var actual = await sut.GetAll(default);
        // Assert
        actual.As<OkObjectResult>().Value.Should().BeEquivalentTo(expected.ProviderTypes);
        mediatorMock.Verify(m => m.Send(It.IsAny<GetAllProviderTypesQuery>(), default), Times.Once);
    }
}
