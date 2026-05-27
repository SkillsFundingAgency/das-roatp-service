using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.Api.Models;
using SFA.DAS.RoATPService.Ukrlp.Client;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers;

public class UkrlpLookupControllerTests
{
    [Test, MoqAutoData]
    public async Task When_Calling_UkrlpLookup_Invokes_UkrlpService_Returns_Ok(
        int ukprn,
        UkrlpResponse ukrlpResponse,
        [Frozen] Mock<IUkrlpService> mockService,
        [Greedy] UkrlpLookupController sut,
        CancellationToken cancellationToken)
    {
        mockService
            .Setup(service => service.GetProviderDataAsync(It.IsAny<UkrlpRequest>(), cancellationToken))
            .ReturnsAsync(ukrlpResponse);

        var result = await sut.UkrlpLookup(ukprn, cancellationToken);

        result.As<OkObjectResult>().Should().NotBeNull();
        var actualResult = result.As<OkObjectResult>().Value.As<UkrlpLookupModel>();
        actualResult.Success.Should().Be(ukrlpResponse.Success);
        mockService.Verify(service => service.GetProviderDataAsync(It.Is<UkrlpRequest>(r => r.Ukprns.Contains(ukprn)), cancellationToken), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task When_Calling_UkrlpLookup_Invokes_UkrlpService_OnSuccess_Returns_Data(
            int ukprn,
            Ukrlp.Client.Provider provider,
            [Frozen] Mock<IUkrlpService> mockService,
            [Greedy] UkrlpLookupController sut,
            CancellationToken cancellationToken)
    {
        UkrlpResponse ukrlpResponse = new(true, [provider]);
        ProviderDetails expected = provider;

        mockService
            .Setup(service => service.GetProviderDataAsync(It.IsAny<UkrlpRequest>(), cancellationToken))
            .ReturnsAsync(ukrlpResponse);

        var result = await sut.UkrlpLookup(ukprn, cancellationToken);

        var model = result.As<OkObjectResult>().Value.As<UkrlpLookupModel>();
        model.Results.Should().HaveCount(1);
        model.Results.First().Should().BeEquivalentTo(expected);
    }
}
