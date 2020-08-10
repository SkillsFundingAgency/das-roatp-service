using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.Interfaces;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers
{
    public class FatDataExportControllerTests
    {
        [Test]
        public async Task When_Calling_GetDataExport_Then_Returns_Data_From_Repository()
        {
            var responseFromRepo = new List<object>
            {
                new {something = 1, another = 2},
                new {something = 2, another = 3},
                new {something = 3, another = 4}
            };
            var mockRepository = new Mock<IFatDataExportRepository>();
            mockRepository
                .Setup(repository => repository.GetFatDataExport())
                .ReturnsAsync(responseFromRepo);

            var controller = new FatDataExportController(
                Mock.Of<ILogger<FatDataExportController>>(), 
                mockRepository.Object);

            var result = await controller.DataExport() as OkObjectResult;

            result.Value.Should().Be(responseFromRepo);
        }

        [Test]
        public async Task When_Calling_GetDataExport_And_SqlException_Then_Returns_NoContent()
        {
            var exception = FormatterServices.GetUninitializedObject(typeof(SqlException)) as SqlException; // creating instance of sealed class with private ctor

            var mockRepository = new Mock<IFatDataExportRepository>();
            mockRepository
                .Setup(repository => repository.GetFatDataExport())
                .ThrowsAsync(exception);

            var controller = new FatDataExportController(
                Mock.Of<ILogger<FatDataExportController>>(), 
                mockRepository.Object);

            var result = await controller.DataExport() as NoContentResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }
    }
}