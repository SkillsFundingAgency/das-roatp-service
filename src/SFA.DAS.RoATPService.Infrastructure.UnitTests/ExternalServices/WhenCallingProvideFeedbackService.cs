using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.RoATPService.Domain.Models.ProvideFeedback;
using SFA.DAS.RoATPService.Infrastructure.ExternalServices;
using SFA.DAS.RoATPService.Infrastructure.Interfaces;
using SFA.DAS.RoATPService.Settings;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Infrastructure.UnitTests.ExternalServices
{
    public class WhenCallingProvideFeedbackService
    {
        [Test, MoqAutoData]
        public async Task Then_The_Endpoint_Is_Called_To_Retrieve_All_The_Data(
            string authToken,
            string identifier,
            WebConfiguration webConfiguration,
            List<EmployerFeedbackSourceDto> apiResponse,
            Mock<IAzureClientCredentialHelper> azureClientCredentialHelper)
        {
            //Arrange
            var baseUrl = "https://test.local";
            var configuration = new ProvideFeedbackApiConfiguration
            {
                Url = baseUrl,
                Identifier = identifier
            };
            azureClientCredentialHelper.Setup(x => x.GetAccessTokenAsync(configuration.Identifier)).ReturnsAsync(authToken);
            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(apiResponse)),
                StatusCode = HttpStatusCode.Accepted
            };
            webConfiguration.ProvideFeedbackApiConfiguration = configuration;
            var expectedUrl = $"{baseUrl}/api/feedback";
                
            var httpMessageHandler = SetupMessageHandlerMock(response, new Uri(expectedUrl));
            var client = new HttpClient(httpMessageHandler.Object);
            var service = new ProvideFeedbackService(webConfiguration, client, azureClientCredentialHelper.Object);

            //Act
            var actual = await service.GetProviderFeedBack();
            
            //Assert
            actual.Should().BeAssignableTo<IEnumerable<EmployerFeedbackSourceDto>>();
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Get)
                        && c.RequestUri.AbsoluteUri.Equals(expectedUrl)
                        && c.Headers.Authorization.Scheme.Equals("Bearer")
                        && c.Headers.Authorization.Parameter.Equals(authToken)),
                    ItExpr.IsAny<CancellationToken>()
                );
            actual.Should().BeEquivalentTo(apiResponse);
        }

        [Test, MoqAutoData]
        public async Task Then_If_The_Request_Is_Local_Then_Authentication_Is_Not_Added(
            WebConfiguration webConfiguration,
            List<EmployerFeedbackSourceDto> apiResponse,
            Mock<IAzureClientCredentialHelper> azureClientCredentialHelper)
        {
            //Arrange
            var baseUrl = "https://localhost";
            var configuration = new ProvideFeedbackApiConfiguration
            {
                Url = baseUrl
            };
            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(apiResponse)),
                StatusCode = HttpStatusCode.Accepted
            };
            webConfiguration.ProvideFeedbackApiConfiguration = configuration;
            var expectedUrl = $"{baseUrl}/api/feedback";
                
            var httpMessageHandler = SetupMessageHandlerMock(response, new Uri(expectedUrl));
            var client = new HttpClient(httpMessageHandler.Object);
            var service = new ProvideFeedbackService(webConfiguration, client, azureClientCredentialHelper.Object);

            //Act
            await service.GetProviderFeedBack();
            
            //Assert
            azureClientCredentialHelper.Verify(x => x.GetAccessTokenAsync(It.IsAny<string>()), Times.Never);
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Get)
                        && c.RequestUri.AbsoluteUri.Equals(expectedUrl)),
                    ItExpr.IsAny<CancellationToken>()
                );
        }
        
        [Test, MoqAutoData]
        public async Task Then_If_Returns_Bad_Request_An_Exception_Is_Thrown(
            WebConfiguration webConfiguration,
            Mock<IAzureClientCredentialHelper> azureClientCredentialHelper)
        {
            //Arrange
            var baseUrl = "https://localhost";
            var configuration = new ProvideFeedbackApiConfiguration
            {
                Url = baseUrl
            };
            var response = new HttpResponseMessage
            {
                Content = new StringContent(""),
                StatusCode = HttpStatusCode.BadRequest
            };
            webConfiguration.ProvideFeedbackApiConfiguration = configuration;
            var expectedUrl = $"{baseUrl}/api/feedback";
                
            var httpMessageHandler = SetupMessageHandlerMock(response, new Uri(expectedUrl));
            var client = new HttpClient(httpMessageHandler.Object);
            var service = new ProvideFeedbackService(webConfiguration, client, azureClientCredentialHelper.Object);

            //Act Assert
            Assert.ThrowsAsync<HttpRequestException>(()=> service.GetProviderFeedBack());
        }
        
        private static Mock<HttpMessageHandler> SetupMessageHandlerMock(HttpResponseMessage response, Uri uri)
        {
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Get)
                        && c.RequestUri.Equals(uri)
                    ),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) => response);
            return httpMessageHandler;
        }
    }
}