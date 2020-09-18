using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Domain.Models.FatDataExport;
using SFA.DAS.RoATPService.Domain.Models.ProvideFeedback;
using SFA.DAS.RoATPService.Infrastructure.Interfaces;
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

        [Test, MoqAutoData]
        public async Task Then_The_ProvideFeedbackService_Is_Called_And_Data_Returned(
            List<FatDataExportDto> data,
            List<EmployerFeedbackSourceDto> serviceData,
            [Frozen] Mock<IFatDataExportRepository> repository,
            [Frozen] Mock<IProvideFeedbackService> feedbackService,
            FatDataExportService service)
        {
            //Arrange
            var ukPrn = data.First().UkPrn;
            serviceData = serviceData.Select(c =>
            {
                c.Ukprn = ukPrn;
                c.ProviderRating = "Good";
                return c;
            }).ToList();
            feedbackService.Setup(x => x.GetProviderFeedBack())
                .ReturnsAsync(serviceData);
            repository
                .Setup(r => r.GetFatDataExport())
                .ReturnsAsync(data);
            
            //Act
            var actual = (await service.GetData()).ToList();
            
            //Assert
            feedbackService.Verify(x=>x.GetProviderFeedBack(), Times.Once);
            var actualDataExport = actual.FirstOrDefault(c => c.UkPrn.Equals(ukPrn));
            Assert.IsNotNull(actualDataExport);
            actualDataExport.Feedback.Total.Should().Be(serviceData.Count);
            actualDataExport.Feedback.FeedbackRating.ContainsKey("Good").Should().BeTrue();
            actualDataExport.Feedback.FeedbackRating["Good"].Should().Be(serviceData.Count);
        }

        [Test, MoqAutoData]
        public async Task Then_The_FeedbackData_Is_Grouped_For_Attributes_On_Strengths_And_Weaknesses(
            List<FatDataExportDto> data,
            [Frozen] Mock<IFatDataExportRepository> repository,
            [Frozen] Mock<IProvideFeedbackService> feedbackService,
            FatDataExportService service)
        {
            //Arrange
            var serviceData = new List<EmployerFeedbackSourceDto>();
                
            var ukPrn = data.First().UkPrn;
            serviceData.Add(new EmployerFeedbackSourceDto
            {
                Ukprn = ukPrn,
                ProviderRating = "Good",
                ProviderAttributes = new List<ProviderAttributeSourceDto>
                {
                    new ProviderAttributeSourceDto
                    {
                        Name = "One",
                        Value = -1
                    },
                    new ProviderAttributeSourceDto
                    {
                        Name = "Two",
                        Value = 1
                    },
                    new ProviderAttributeSourceDto
                    {
                        Name = "Three",
                        Value = -1
                    }
                }
            });
            serviceData.Add(new EmployerFeedbackSourceDto
            {
                Ukprn = ukPrn,
                ProviderRating = "Good",
                ProviderAttributes = new List<ProviderAttributeSourceDto>
                {
                    new ProviderAttributeSourceDto
                    {
                        Name = "One",
                        Value = 1
                    },
                    new ProviderAttributeSourceDto
                    {
                        Name = "Two",
                        Value = 1
                    },
                    new ProviderAttributeSourceDto
                    {
                        Name = "Three",
                        Value = -1
                    }
                }
            });
            feedbackService.Setup(x => x.GetProviderFeedBack())
                .ReturnsAsync(serviceData);
            repository
                .Setup(r => r.GetFatDataExport())
                .ReturnsAsync(data);
            
            //Act
            var actual = (await service.GetData()).ToList();
            
            //Assert
            var actualData = actual.FirstOrDefault(c => c.UkPrn.Equals(ukPrn));
            Assert.IsNotNull(actualData);
            actualData.Feedback.ProviderAttributes.Count.Should().Be(3);
            var feedBackOne = actualData.Feedback
                .ProviderAttributes.FirstOrDefault(c => c.Name.Equals("One"));
            Assert.IsNotNull(feedBackOne);
            feedBackOne.Strengths.Should().Be(1);
            feedBackOne.Weaknesses.Should().Be(1);
            var feedBackTwo = actualData.Feedback
                .ProviderAttributes.FirstOrDefault(c => c.Name.Equals("Two"));
            Assert.IsNotNull(feedBackTwo);
            feedBackTwo.Strengths.Should().Be(2);
            feedBackTwo.Weaknesses.Should().Be(0);
            var feedBackThree = actualData.Feedback
                .ProviderAttributes.FirstOrDefault(c => c.Name.Equals("Three"));
            Assert.IsNotNull(feedBackThree);
            feedBackThree.Strengths.Should().Be(0);
            feedBackThree.Weaknesses.Should().Be(2);
        }
    }
}