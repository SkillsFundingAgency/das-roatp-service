using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;

namespace SFA.DAS.RoATPService.Api.Client.IntegrationTests
{
    using System.Linq;
    using System.Net.Http;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using SFA.DAS.RoATPService.Api.Client.AutoMapper;
    using SFA.DAS.RoATPService.Settings;

    [TestFixture]
    public class UkrlpLookupTests
    {
        private Mock<ILogger<UkrlpApiClient>> _logger;
        private Mock<WebConfiguration> _config;

        [SetUp]
        public void Before_each_test()
        {
            Mapper.Reset();

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<UkrlpVerificationDetailsProfile>();
                cfg.AddProfile<UkrlpContactPersonalDetailsProfile>();
                cfg.AddProfile<UkrlpContactAddressProfile>();
                cfg.AddProfile<UkrlpProviderAliasProfile>();
                cfg.AddProfile<UkrlpProviderContactProfile>();
                cfg.AddProfile<UkrlpProviderDetailsProfile>();
            });

            Mapper.AssertConfigurationIsValid();

            _logger = new Mock<ILogger<UkrlpApiClient>>();
            _config = new Mock<WebConfiguration>();
            var apiConfig = new UkrlpApiAuthentication
            {
                QueryId = "2",
                StakeholderId = "2",
                ApiBaseAddress = "http://webservices.ukrlp.co.uk/UkrlpProviderQueryWS6/ProviderQueryServiceV6"
            };

            _config.SetupGet(x => x.UkrlpApiAuthentication).Returns(apiConfig);
        }

        [Test]
        public void Matching_UKPRN_returns_single_result()
        {
            var ukprn = 10012385;
            var client = new UkrlpApiClient(_logger.Object, _config.Object, new HttpClient(),
                new UkrlpSoapSerializer());

            var result = client.GetTrainingProviderByUkprn(ukprn).GetAwaiter().GetResult();

            result.Should().NotBeNull();
            result.Results.Count.Should().Be(1);
            var matchResult = result.Results[0];
            matchResult.UKPRN.Should().Be(ukprn.ToString());
            matchResult.ProviderStatus.Should().Be("Active");
            matchResult.ContactDetails.Find(x => x.ContactType == "L").Should().NotBeNull();
            matchResult.VerificationDate.Should().NotBeNull();
            matchResult.VerificationDetails
                .Find(x => x.VerificationAuthority == "Charity Commission")
                .Should().NotBeNull();
            matchResult.ProviderAliases.Count.Should().Be(1);
        }

        [Test]
        public void Matching_UKPRN_has_a_primary_verification_source()
        {
            var ukprn = 10006287;
            var client = new UkrlpApiClient(_logger.Object, _config.Object, new HttpClient(),
                new UkrlpSoapSerializer());

            var result = client.GetTrainingProviderByUkprn(ukprn).GetAwaiter().GetResult();

            result.Should().NotBeNull();
            result.Results.Count.Should().Be(1);
            var matchResult = result.Results[0];
            matchResult.UKPRN.Should().Be(ukprn.ToString());
            var primaryVerification =
                matchResult.VerificationDetails.FirstOrDefault(x => x.PrimaryVerificationSource == true);
            primaryVerification.VerificationAuthority.Should().Be("Charity Commission");
        }

        [Test]
        public void Non_matching_UKPRN_returns_no_results()
        {
            var ukprn = 99998888;

            var client = new UkrlpApiClient(_logger.Object, _config.Object, new HttpClient(),
                new UkrlpSoapSerializer());

            var result = client.GetTrainingProviderByUkprn(ukprn).GetAwaiter().GetResult();

            result.Should().NotBeNull();
            result.Results.Count.Should().Be(0);
        }

        [Test]
        public void Inactive_UKPRN_returns_no_results()
        {
            var ukprn = 10019227;

            var client = new UkrlpApiClient(_logger.Object, _config.Object, new HttpClient(),
                new UkrlpSoapSerializer());

            var result = client.GetTrainingProviderByUkprn(ukprn).GetAwaiter().GetResult();

            result.Should().NotBeNull();
            result.Results.Count.Should().Be(0);
        }

        [Test]
        public async Task Then_Returns_Multiple_Ukprns()
        {
            //Arrange
            var ukprns = new List<long> { 10012385, 10006287 };
            var client = new UkrlpApiClient(_logger.Object, _config.Object, new HttpClient(),
                new UkrlpSoapSerializer());

            //Act
            var result = await client.GetListOfTrainingProviders(ukprns);

            result.Results.Count.Should().Be(2);
            result.Results.Select(c => c.UKPRN).Should().Contain(ukprns.Select(c => c.ToString()));
        }
    }
}