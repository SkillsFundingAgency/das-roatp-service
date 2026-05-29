using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace SFA.DAS.RoATPService.Ukrlp.Client.UnitTests;

[TestFixture]
public class UkrlpServiceTests
{
    private Mock<HttpMessageHandler> _mockHandler;
    private HttpClient _httpClient;
    private UkrlpService _sut;

    [SetUp]
    public void SetUp()
    {
        _mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        // Base address is required since the service uses relative paths (api/providers)
        _httpClient = new HttpClient(_mockHandler.Object, false)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _sut = new UkrlpService(_httpClient);
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
    }

    [Test]
    public async Task WhenGettingDataFromUkrlp_AppendsQueryParamsCorrectly()
    {
        // Arrange
        var request = new UkrlpQuery(new DateTime(2026, 05, 29, 0, 0, 0, DateTimeKind.Unspecified), [10012001, 10012002]);
        var responseObj = new UkrlpResponse([]);

        SetupMockHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(responseObj));

        // Act
        await _sut.GetProviderDataAsync(request, CancellationToken.None);

        // Assert
        // Verifies the date is correctly formatted as yyyy-MM-dd in the query string
        VerifyHttpCall(HttpMethod.Get, "api/providers?ukprns=10012001&ukprns=10012002&updatedSince=2026-05-29");
    }

    [Test]
    public async Task WhenGettingDataFromUkrlp_AndHttpCallFails_ReturnsUnsuccessfulResult()
    {
        // Arrange
        var request = new UkrlpQuery(null, [12345678]);

        SetupMockHttpResponse(HttpStatusCode.InternalServerError, string.Empty);

        // Act
        var result = await _sut.GetProviderDataAsync(request, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Providers, Is.Empty);
    }

    [Test, AutoData]
    public async Task WhenGettingDataFromUkrlp_OnSuccessfulResponse_ParsesNdjsonCorrectly(Provider provider1, Provider provider2)
    {
        // Arrange
        provider1.UKPRN = "10012001";
        provider2.UKPRN = "10005678";
        var request = new UkrlpQuery(null, [12345678]);

        // Simulate NDJSON with two separate JSON objects on new lines
        var response1 = new UkrlpResponse([provider1]);
        var response2 = new UkrlpResponse([provider2]);

        var ndjsonBuilder = new StringBuilder();
        ndjsonBuilder.AppendLine(JsonSerializer.Serialize(response1));
        ndjsonBuilder.AppendLine(JsonSerializer.Serialize(response2));

        SetupMockHttpResponse(HttpStatusCode.OK, ndjsonBuilder.ToString());

        // Act
        var result = await _sut.GetProviderDataAsync(request, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);

        var providers = result.Providers.ToList();
        Assert.That(providers, Has.Count.EqualTo(2));

        Assert.That(providers, Has.Some.Matches<Provider>(p => p.UKPRN == provider1.UKPRN && p.ProviderName == provider1.ProviderName));
        Assert.That(providers, Has.Some.Matches<Provider>(p => p.UKPRN == provider2.UKPRN && p.ProviderName == provider2.ProviderName));
    }

    private void SetupMockHttpResponse(HttpStatusCode statusCode, string content)
    {
        var response = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(content, Encoding.UTF8, "application/x-ndjson")
        };

        _mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);
    }

    private void VerifyHttpCall(HttpMethod method, string expectedRelativeUrl)
    {
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == method &&
                req.RequestUri!.PathAndQuery.TrimStart('/') == expectedRelativeUrl.TrimStart('/')),
            ItExpr.IsAny<CancellationToken>()
        );
    }
}
