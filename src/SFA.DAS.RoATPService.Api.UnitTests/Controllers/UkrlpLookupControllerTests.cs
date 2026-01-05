using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Ukrlp.Client.Interfaces;
using SFA.DAS.RoATPService.Ukrlp.Client.Models;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers;
public class UkrlpLookupControllerTests
{
    [Test]
    public async Task When_Calling_UkrlpLookup_Then_Returns_Data_From_Client()
    {
        var ukprn = 1;
        var response = new UkprnLookupResponse
        {
            Success = true
        };
        var mockService = new Mock<IUkrlpApiClient>();
        mockService
            .Setup(service => service.GetTrainingProviderByUkprn(ukprn))
            .ReturnsAsync(response);

        var controller = new UkrlpLookupController(
            Mock.Of<ILogger<UkrlpLookupController>>(),
            mockService.Object);


        var result = await controller.UkrlpLookup(ukprn) as OkObjectResult;

        result.Value.Should().Be(response);
        var actualResult = result.Value as UkprnLookupResponse;
        actualResult.Success.Should().BeTrue();
    }

    [Test]
    public async Task When_Calling_UkrlpLookup_And_Exception_Then_Returns_ErrorResponse()
    {
        int ukprn = 1;
        var mockService = new Mock<IUkrlpApiClient>();
        mockService
            .Setup(service => service.GetTrainingProviderByUkprn(It.IsAny<long>()))
            .ThrowsAsync(new Exception());

        var controller = new UkrlpLookupController(
            Mock.Of<ILogger<UkrlpLookupController>>(),
            mockService.Object);

        var result = await controller.UkrlpLookup(ukprn) as OkObjectResult;

        var actualResult = result.Value as UkprnLookupResponse;
        actualResult.Success.Should().BeFalse();
        actualResult.Results.Should().BeEmpty();
    }
}
