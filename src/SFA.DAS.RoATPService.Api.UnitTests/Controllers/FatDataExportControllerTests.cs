using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Domain.Models.FatDataExport;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers
{
    public class FatDataExportControllerTests
    {
        [Test]
        public async Task When_Calling_GetDataExport_Then_Returns_Data_From_Repository()
        {
            var responseFromRepo = new List<FatDataExport>
            {
                new FatDataExport()
            };
            var mockService = new Mock<IFatDataExportService>();
            mockService
                .Setup(service => service.GetData())
                .ReturnsAsync(responseFromRepo);

            var controller = new FatDataExportController(
                Mock.Of<ILogger<FatDataExportController>>(), 
                mockService.Object);

            var result = await controller.DataExport() as OkObjectResult;

            result.Value.Should().Be(responseFromRepo);
        }

        [Test]
        public async Task When_Calling_GetDataExport_And_Exception_Then_Returns_NoContent()
        {
            var mockService = new Mock<IFatDataExportService>();
            mockService
                .Setup(service => service.GetData())
                .ThrowsAsync(new Exception());

            var controller = new FatDataExportController(
                Mock.Of<ILogger<FatDataExportController>>(), 
                mockService.Object);

            var result = await controller.DataExport() as StatusCodeResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }
}