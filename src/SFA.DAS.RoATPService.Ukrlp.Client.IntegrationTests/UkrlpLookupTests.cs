using System.Linq;
using System.Net.Http;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Ukrlp.Client;

namespace SFA.DAS.RoATPService.Api.Client.IntegrationTests;

[TestFixture]
public class UkrlpLookupTests
{
    private Mock<ILogger<UkrlpApiClient>> _logger;
    private UkrlpApiAuthentication _config;

    [SetUp]
    public void Before_each_test()
    {
        _logger = new Mock<ILogger<UkrlpApiClient>>();
        _config = new UkrlpApiAuthentication()
        {
            QueryId = "2",
            StakeholderId = "2",
            ApiBaseAddress = "http://webservices.ukrlp.co.uk/UkrlpProviderQueryWS6/ProviderQueryServiceV6"
        };
    }

    [Test]
    public void Matching_UKPRN_returns_single_result()
    {
        var ukprn = 10012385;
        var client = new UkrlpApiClient(_logger.Object, _config, new HttpClient(),
            new UkrlpSoapSerializer());

        var result = client.GetTrainingProviderByUkprn(ukprn).GetAwaiter().GetResult();

        result.Should().NotBeNull();
        result.Results.Count.Should().Be(1);
        var matchResult = result.Results[0];
        matchResult.UKPRN.Should().Be(ukprn.ToString());
        matchResult.ProviderStatus.Should().Be("Active");
        matchResult.ContactDetails.Select(x => x.ContactType == "L").Should().NotBeNull();
        matchResult.VerificationDate.Should().NotBeNull();
        matchResult.VerificationDetails
            .Select(x => x.VerificationAuthority == "Charity Commission")
            .Should().NotBeNull();
        matchResult.ProviderAliases.Count().Should().Be(1);
    }

    [Test]
    public void Matching_UKPRN_has_a_primary_verification_source()
    {
        var ukprn = 10006287;
        var client = new UkrlpApiClient(_logger.Object, _config, new HttpClient(),
            new UkrlpSoapSerializer());

        var result = client.GetTrainingProviderByUkprn(ukprn).GetAwaiter().GetResult();

        result.Should().NotBeNull();
        result.Results.Count.Should().Be(1);
        var matchResult = result.Results[0];
        matchResult.UKPRN.Should().Be(ukprn.ToString());
        var primaryVerification =
            matchResult.VerificationDetails.First(x => x.PrimaryVerificationSource);
        primaryVerification.VerificationAuthority.Should().Be("Charity Commission");
    }

    [Test]
    public void Non_matching_UKPRN_returns_no_results()
    {
        var ukprn = 99998888;

        var client = new UkrlpApiClient(_logger.Object, _config, new HttpClient(),
            new UkrlpSoapSerializer());

        var result = client.GetTrainingProviderByUkprn(ukprn).GetAwaiter().GetResult();

        result.Should().NotBeNull();
        result.Results.Count.Should().Be(0);
    }

    [Test]
    public void Inactive_UKPRN_returns_no_results()
    {
        var ukprn = 10019227;

        var client = new UkrlpApiClient(_logger.Object, _config, new HttpClient(),
            new UkrlpSoapSerializer());

        var result = client.GetTrainingProviderByUkprn(ukprn).GetAwaiter().GetResult();

        result.Should().NotBeNull();
        result.Results.Count.Should().Be(0);
    }
}
