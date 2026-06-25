using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace SFA.DAS.RoATPService.Ukrlp.Client.UnitTests;

[TestFixture]
[NonParallelizable]
public class OAuthTokenServiceTests
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory;
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private Mock<ILogger<OAuthTokenService>> _mockLogger;
    private UkrlpApiAuthentication _options;
    private OAuthTokenService _sut;

    [SetUp]
    public void SetUp()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockLogger = new Mock<ILogger<OAuthTokenService>>();

        // Setup options
        _options = new UkrlpApiAuthentication
        {
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            Scope = "test-scope",
            TokenEndpoint = "https://identity.local"
        };

        // Create a new client each time
        _mockHttpClientFactory
            .Setup(f => f.CreateClient(Constants.TokenClientName))
            .Returns(() => new HttpClient(_mockHttpMessageHandler.Object, disposeHandler: false));

        _sut = new OAuthTokenService(_mockHttpClientFactory.Object, _options, _mockLogger.Object);
    }

    [Test]
    public async Task WhenFetchingAndCachingToken_SendsCorrectFormUrlEncodedBody()
    {
        // Arrange
        SetupMockTokenResponse(HttpStatusCode.OK, new TokenResponse("token", 3600, "Bearer"));
        HttpRequestMessage interceptedRequest = null!;

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, ct) => interceptedRequest = req)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new TokenResponse("token", 3600, "Bearer"))
            });

        // Act
        await _sut.GetAccessTokenAsync();

        // Assert
        Assert.That(interceptedRequest, Is.Not.Null);
        var bodyContent = await interceptedRequest.Content!.ReadAsStringAsync();

        Assert.That(bodyContent, Contains.Substring("grant_type=client_credentials"));
        Assert.That(bodyContent, Contains.Substring($"client_id={_options.ClientId}"));
        Assert.That(bodyContent, Contains.Substring($"client_secret={_options.ClientSecret}"));
        Assert.That(bodyContent, Contains.Substring($"scope={_options.Scope}"));
    }


    [Test]
    public async Task WhenGettingAccessToken_FirstCall_FetchesTokenFromEndpoint()
    {
        // Arrange
        var expectedToken = "token_abc_123";
        SetupMockTokenResponse(HttpStatusCode.OK, new TokenResponse(expectedToken, 3600, "Bearer"));

        // Act
        var token = await _sut.GetAccessTokenAsync();

        // Assert
        Assert.That(token, Is.EqualTo(expectedToken));
        VerifyHttpCallCount(Times.Once());
    }

    [Test]
    public async Task WhenGettingAccessToken_ConsecutiveCallsWithinExpiry_ReturnsCachedTokenWithoutHttpCall()
    {
        // Arrange
        var expectedToken = "cached_token_xyz";
        SetupMockTokenResponse(HttpStatusCode.OK, new TokenResponse(expectedToken, 3600, "Bearer"));

        // Act
        var token1 = await _sut.GetAccessTokenAsync();
        var token2 = await _sut.GetAccessTokenAsync();

        // Assert
        Assert.That(token1, Is.EqualTo(expectedToken));
        Assert.That(token2, Is.EqualTo(expectedToken));
        VerifyHttpCallCount(Times.Once()); // Network called only once
    }

    [Test]
    public async Task WhenGettingAccessToken_TokenExpiredBasedOnBuffer_FetchesNewToken()
    {
        // Arrange
        var expiredToken = "expired_token";
        var freshToken = "fresh_token";

        _mockHttpMessageHandler
            .Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Returns(() => Task.Run(() => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new TokenResponse(expiredToken, 20, "Bearer"))
            }))
            .Returns(() => Task.Run(() => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new TokenResponse(freshToken, 3600, "Bearer"))
            }));

        // Act & Advance Time contextually
        var token1 = await _sut.GetAccessTokenAsync();

        // Wait 6 seconds to clear the effective 5-second buffer expiry window
        await Task.Delay(6000);

        await Task.Yield();

        var token2 = await _sut.GetAccessTokenAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(token1, Is.EqualTo(expiredToken));
            Assert.That(token2, Is.EqualTo(freshToken));
            VerifyHttpCallCount(Times.Exactly(2));
        });
    }

    [Test]
    public async Task WhenGettingAccessToken_ConcurrentRequests_ReusesInFlightTask()
    {
        // Arrange
        var expectedToken = "concurrent_token";
        var tcs = new TaskCompletionSource<HttpResponseMessage>();

        // Hold the response in-flight to simulate delay
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Returns(tcs.Task);

        // Act
        var task1 = _sut.GetAccessTokenAsync();
        var task2 = _sut.GetAccessTokenAsync();

        // Release the HTTP response
        tcs.SetResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new TokenResponse(expectedToken, 3600, "Bearer"))
        });

        var token1 = await task1;
        var token2 = await task2;

        // Assert
        Assert.That(token1, Is.EqualTo(expectedToken));
        Assert.That(token2, Is.EqualTo(expectedToken));
        VerifyHttpCallCount(Times.Once());
    }

    [Test]
    public async Task WhenInvalidatingAccessToken_ClearsCache_ForcesNetworkFetchOnNextCall()
    {
        // Arrange
        var initialToken = "token_one";
        var newToken = "token_two";

        var response1 = new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(new TokenResponse(initialToken, 3600, "Bearer")) };
        var response2 = new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(new TokenResponse(newToken, 3600, "Bearer")) };

        _mockHttpMessageHandler
            .Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        // Act
        var token1 = await _sut.GetAccessTokenAsync();
        _sut.InvalidateAccessToken();
        var token2 = await _sut.GetAccessTokenAsync();

        // Assert
        Assert.That(token1, Is.EqualTo(initialToken));
        Assert.That(token2, Is.EqualTo(newToken));
        VerifyHttpCallCount(Times.Exactly(2));
    }

    #region Helpers

    private void SetupMockTokenResponse(HttpStatusCode statusCode, TokenResponse responseObj)
    {
        var response = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = JsonContent.Create(responseObj)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);
    }

    private void VerifyHttpCallCount(Times times)
    {
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            times,
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    #endregion
}
