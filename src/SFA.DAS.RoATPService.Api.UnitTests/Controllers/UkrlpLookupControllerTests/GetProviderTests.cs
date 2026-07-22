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

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers.UkrlpLookupControllerTests;

public class GetProviderTests
{
    [Test, MoqAutoData]
    public async Task WhenCallingUkrlpLookup_OnSuccessfulResponseFromUkrlp_ReturnsData(
        int ukprn,
        Ukrlp.Client.Provider provider,
        [Frozen] Mock<IUkrlpService> mockService,
        [Greedy] UkrlpLookupController sut,
        CancellationToken cancellationToken)
    {
        UkrlpQueryResult ukrlpResponse = new(true, [provider]);
        provider.UKPRN = "10012002";
        ProviderModel expected = provider;

        mockService
            .Setup(service => service.GetProviderDataAsync(It.IsAny<UkrlpQuery>(), cancellationToken))
            .ReturnsAsync(ukrlpResponse);

        var result = await sut.GetProvider(ukprn, cancellationToken);

        var model = result.As<OkObjectResult>().Value.As<ProviderModel>();
        model.Should().NotBeNull();
        model.Should().BeEquivalentTo(expected);
    }

    [Test, MoqAutoData]
    public async Task WhenCallingUkrlpLookup_UkprnNotFound_ReturnsNotFound(
        int ukprn,
        [Frozen] Mock<IUkrlpService> mockService,
        [Greedy] UkrlpLookupController sut,
        CancellationToken cancellationToken)
    {
        mockService
            .Setup(service => service.GetProviderDataAsync(It.IsAny<UkrlpQuery>(), cancellationToken))
            .ReturnsAsync(new UkrlpQueryResult(true, []));

        var result = await sut.GetProvider(ukprn, cancellationToken);

        result.Should().BeOfType<NotFoundResult>();
        mockService.Verify(service => service.GetProviderDataAsync(It.Is<UkrlpQuery>(r => r.Ukprns.Contains(ukprn)), cancellationToken), Times.Once);
    }
}
