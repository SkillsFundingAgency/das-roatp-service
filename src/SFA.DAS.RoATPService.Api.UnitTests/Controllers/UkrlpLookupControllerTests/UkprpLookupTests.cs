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

public class UkprpLookupTests
{
    [Test, MoqAutoData]
    public async Task WhenCallingUkrlpLookup_InvokesUkrlpService_ReturnsOk(
        int ukprn,
        UkrlpQueryResult ukrlpResponse,
        [Frozen] Mock<IUkrlpService> mockService,
        [Greedy] UkrlpLookupController sut,
        CancellationToken cancellationToken)
    {
        mockService
            .Setup(service => service.GetProviderDataAsync(It.IsAny<UkrlpQuery>(), cancellationToken))
            .ReturnsAsync(ukrlpResponse);

        var result = await sut.UkrlpLookup(ukprn, cancellationToken);

        result.As<OkObjectResult>().Should().NotBeNull();
        var actualResult = result.As<OkObjectResult>().Value.As<UkrlpLookupModel>();
        actualResult.Success.Should().Be(ukrlpResponse.Success);
        mockService.Verify(service => service.GetProviderDataAsync(It.Is<UkrlpQuery>(r => r.Ukprns.Contains(ukprn)), cancellationToken), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenCallingUkrlpLookup_OnSuccessfulResponseFromUkrlp_ReturnsData(
            int ukprn,
            Ukrlp.Client.Provider provider,
            [Frozen] Mock<IUkrlpService> mockService,
            [Greedy] UkrlpLookupController sut,
            CancellationToken cancellationToken)
    {
        UkrlpQueryResult ukrlpResponse = new(true, [provider]);
        ProviderDetails expected = provider;

        mockService
            .Setup(service => service.GetProviderDataAsync(It.IsAny<UkrlpQuery>(), cancellationToken))
            .ReturnsAsync(ukrlpResponse);

        var result = await sut.UkrlpLookup(ukprn, cancellationToken);

        var model = result.As<OkObjectResult>().Value.As<UkrlpLookupModel>();
        model.Results.Should().HaveCount(1);
        model.Results.First().Should().BeEquivalentTo(expected);
    }

    [Test]
    [MoqInlineAutoData("ISC", "Independent Schools Council")]
    [MoqInlineAutoData("SCC", "Scottish Executive Education Department")]
    [MoqInlineAutoData("CHARITY", "Charity Commission")]
    [MoqInlineAutoData("URN", "DfE(Schools Unique Reference Number)")]
    [MoqInlineAutoData("NII", "Department of Education in Northern Ireland")]
    [MoqInlineAutoData("SFA", "SFA Validated")]
    [MoqInlineAutoData("SI", "Government Statute")]
    [MoqInlineAutoData("COMPANY", "Companies House")]
    [MoqInlineAutoData("SOLE", "Sole Trader or Non-limited Partnership")]
    [MoqInlineAutoData("DFES", "DfE(LEA Code and Establishment Number)")]
    [MoqInlineAutoData("OTHER", "Unknown")]
    public async Task WhenCallingUkrlpLookup_OnSuccessfulResponseFromUkrlp_TransformsVerificationAuthorityToOldValues(
        string newValue,
        string expectedOldValue,
        int ukprn,
        Provider provider,
        [Frozen] Mock<IUkrlpService> mockService,
        [Greedy] UkrlpLookupController sut,
        CancellationToken cancellationToken)
    {
        provider.VerificationDetails = new[]
        {
            new Ukrlp.Client.VerificationInfo { VerificationAuthority = newValue },
        };
        UkrlpQueryResult ukrlpResponse = new(true, [provider]);
        mockService
            .Setup(service => service.GetProviderDataAsync(It.IsAny<UkrlpQuery>(), cancellationToken))
            .ReturnsAsync(ukrlpResponse);

        var result = await sut.UkrlpLookup(ukprn, cancellationToken);

        var model = result.As<OkObjectResult>().Value.As<UkrlpLookupModel>();
        model.Results.Should().HaveCount(1);
        model.Results.First().VerificationDetails.Should().ContainSingle(v => v.VerificationAuthority == expectedOldValue);
    }
}
