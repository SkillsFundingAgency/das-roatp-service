using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Api.Client.Interfaces;
using SFA.DAS.RoATPService.Api.Client.Models.Ukrlp;
using SFA.DAS.RoATPService.Application.Api.Controllers;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers
{
    public class UkrlpLookupControllerTests
    {
        [Test]
        public async Task When_Calling_UkrlpGetAll_Then_Returns_Data_From_Client()
        {
            var ukprns = new List<long>{1,2,3};
            var response = new UkprnLookupResponse
            {
                Success = true
            };
            var mockService = new Mock<IUkrlpApiClient>();
            mockService
                .Setup(service => service.GetListOfTrainingProviders(ukprns))
                .ReturnsAsync(response);

            var controller = new UkrlpLookupController(
                Mock.Of<ILogger<UkrlpLookupController>>(), 
                mockService.Object);

            
            var result = await controller.UkrlpGetAll(ukprns) as OkObjectResult;

            result.Value.Should().Be(response);
            var actualResult = result.Value as UkprnLookupResponse;
            actualResult.Success.Should().BeTrue();
        }

        [Test]
        public async Task When_Calling_UkrlpGetAll_And_Exception_Then_Returns_ErrorResponse()
        {
            var mockService = new Mock<IUkrlpApiClient>();
            mockService
                .Setup(service => service.GetListOfTrainingProviders(It.IsAny<List<long>>()))
                .ThrowsAsync(new Exception());

            var controller = new UkrlpLookupController(
                Mock.Of<ILogger<UkrlpLookupController>>(), 
                mockService.Object);

            var result = await controller.UkrlpGetAll(new List<long>()) as OkObjectResult;

            var actualResult = result.Value as UkprnLookupResponse;
            actualResult.Success.Should().BeFalse();
            actualResult.Results.Should().BeEmpty();
        }
    }
}