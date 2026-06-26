using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Models;
using SFA.DAS.RoATPService.Ukrlp.Client;

namespace SFA.DAS.RoATPService.Api.UnitTests.Models.ProviderModelTests;

public class WhenConvertingFromUkrlpRestApiResponse
{
    [Test, AutoData]
    public void WhenConvertingFromProvider_ConvertsUkprnToInt(Provider source)
    {
        source.UKPRN = "10012001";
        // Action
        ProviderModel sut = source;

        Assert.That(sut.Ukprn, Is.EqualTo(int.Parse(source.UKPRN)));
    }

    [Test, AutoData]
    public void WhenConvertingFromProvider_MapsToProviderName(Provider source)
    {
        source.UKPRN = "10012001";

        // Action
        ProviderModel sut = source;

        Assert.That(sut.LegalName, Is.EqualTo(source.ProviderName.ToUpper()));
    }

    [Test, AutoData]
    public void WhenConvertingFromProvider_AssignsLegalNameAfterTrimmingProviderName(Provider source)
    {
        source.UKPRN = "10012001";
        source.ProviderName = "  Test  ";

        // Action
        ProviderModel sut = source;

        Assert.That(sut.LegalName, Is.EqualTo(source.ProviderName.Trim().ToUpper()));
    }

    [Test, AutoData]
    public void WhenConvertingFromProvider_AssignsTradingNameFromFirstAlias(Provider source, string expectedTradingName)
    {
        source.UKPRN = "10012001";
        source.ProviderAliases = [new Ukrlp.Client.ProviderAlias(expectedTradingName), new Ukrlp.Client.ProviderAlias("unexpected")];

        // Action
        ProviderModel sut = source;

        Assert.That(sut.TradingName, Is.EqualTo(expectedTradingName));
    }

    [Test, AutoData]
    public void WhenConvertingFromProvider_MapsContactDetailsFromPrimaryContact(Provider source, Ukrlp.Client.ProviderContact contact, ContactPerson expectedProviderContact, string expectedEmail, string expectedUrl, string expectedPhone)
    {
        source.UKPRN = "10012001";
        source.PrimaryContact = new(expectedProviderContact, null, expectedPhone, null, expectedEmail, expectedUrl, null);

        // Action
        ProviderModel sut = source;

        Assert.Multiple(() =>
        {
            Assert.That(sut.ContactDetails.Title, Is.EqualTo(expectedProviderContact.PersonNameTitle));
            Assert.That(sut.ContactDetails.FirstName, Is.EqualTo(expectedProviderContact.PersonGivenName));
            Assert.That(sut.ContactDetails.LastName, Is.EqualTo(expectedProviderContact.PersonFamilyName));
            Assert.That(sut.ContactDetails.Email, Is.EqualTo(expectedEmail));
            Assert.That(sut.ContactDetails.Website, Is.EqualTo(expectedUrl));
            Assert.That(sut.ContactDetails.Telephone, Is.EqualTo(expectedPhone));
        });
    }

    [Test, AutoData]
    public void WhenConvertingFromProvider_MapsLegalAddressFromLegalAddress(Provider source, Address expectedAddress)
    {
        source.UKPRN = "10012001";
        source.LegalAddress = expectedAddress;

        // Action
        ProviderModel sut = source;

        Assert.That(sut.LegalAddress, Is.EqualTo(expectedAddress));
    }

    [Test, AutoData]
    public void WhenConvertingFromProvider_MapsVerificationDetails(Provider source, IEnumerable<VerificationInfo> expectedVerificationDetails)
    {
        source.UKPRN = "10012001";
        source.VerificationDetails = expectedVerificationDetails;

        // Action
        ProviderModel sut = source;

        sut.VerificationDetails.Should().BeEquivalentTo(expectedVerificationDetails);
    }
}
