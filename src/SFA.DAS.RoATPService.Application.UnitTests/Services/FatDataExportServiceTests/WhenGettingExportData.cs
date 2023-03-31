using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Domain.Models.FatDataExport;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Services.FatDataExportServiceTests
{
    public class WhenGettingExportData
    {
        [Test, MoqAutoData]
        public async Task Then_The_Repository_Is_Called(
            List<FatDataExportDto> data,
            [Frozen] Mock<IFatDataExportRepository> repository,
            FatDataExportService service)
        {
            //Arrange
            repository
                .Setup(r => r.GetFatDataExport())
                .ReturnsAsync(data);
            
            //Act
            var actual = await service.GetData();
            
            //Assert
            actual.Should().BeEquivalentTo(data);
            repository.Verify(x=>x.GetFatDataExport(), Times.Once);
        }
    }
}