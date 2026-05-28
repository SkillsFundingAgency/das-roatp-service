using System.Collections.Generic;
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

public class GetProvidersTests
{
    [Test, MoqAutoData]
    public async Task When_Getting_Single_Provider_From_Ukrlp_Returns_Expected_Provider_Details(
        [Frozen] Mock<IUkrlpService> mockService,
        [Greedy] UkrlpLookupController sut,
        Ukrlp.Client.Provider provider)
    {
        provider.UKPRN = "10012001";
        ProviderModel expected = provider;
        mockService
            .Setup(s => s.GetProviderDataAsync(It.IsAny<UkrlpQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UkrlpQueryResult(true, [provider]));

        var result = await sut.GetProviders([int.Parse(provider.UKPRN)], null, CancellationToken.None);

        result.As<OkObjectResult>().Value.As<UkrlpProvidersModel>().Providers.Should().ContainSingle().Which.Should().BeEquivalentTo(expected);
    }

    [Test, MoqAutoData]
    public async Task When_Getting_Provider_From_Ukrlp_Fails_Returns_InternalServerError(
        [Frozen] Mock<IUkrlpService> mockService,
        [Greedy] UkrlpLookupController sut)
    {
        mockService
            .Setup(s => s.GetProviderDataAsync(It.IsAny<UkrlpQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UkrlpQueryResult(false, []));
        var result = await sut.GetProviders([10012001], null, CancellationToken.None);
        result.As<ObjectResult>().StatusCode.Should().Be(500);
        result.As<ObjectResult>().Value.As<IDictionary<string, string>>().Should().ContainKey("Error").WhoseValue.Should().Be("Failed to retrieve provider data from UKRLP service");
    }
}
