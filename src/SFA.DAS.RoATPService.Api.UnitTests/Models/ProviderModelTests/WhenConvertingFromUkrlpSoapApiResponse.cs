using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Models;
using SFA.DAS.RoATPService.Ukrlp.Client;
using SFA.DAS.RoATPService.Ukrlp.Client.SoapClient;

namespace SFA.DAS.RoATPService.Api.UnitTests.Models.ProviderModelTests;

public class WhenConvertingFromUkrlpSoapApiResponse
{
    [Test, AutoData]
    public void WhenConvertingFromProvider_ConvertsUkprnToInt(MatchingProviderRecords source)
    {
        source.UnitedKingdomProviderReferenceNumber = "10012001";
        source.ProviderContacts = [new ProviderContactStructure() { ContactType = "L" }];
        // Action
        ProviderModel sut = source;

        Assert.That(sut.Ukprn, Is.EqualTo(int.Parse(source.UnitedKingdomProviderReferenceNumber)));
    }

    [Test, AutoData]
    public void WhenConvertingFromProvider_MapsToProviderName(MatchingProviderRecords source)
    {
        source.UnitedKingdomProviderReferenceNumber = "10012001";
        source.ProviderContacts = [new ProviderContactStructure() { ContactType = "L" }];

        // Action
        ProviderModel sut = source;

        Assert.That(sut.LegalName, Is.EqualTo(source.ProviderName.ToUpper()));
    }

    [Test, AutoData]
    public void WhenConvertingFromProvider_AssignsTradingNameFromFirstAlias(MatchingProviderRecords source, string expectedTradingName)
    {
        source.UnitedKingdomProviderReferenceNumber = "10012001";
        source.ProviderContacts = [new ProviderContactStructure() { ContactType = "L" }];
        source.ProviderAliases = [
            new ProviderAliasesStructure() { ProviderAlias = expectedTradingName },
            new ProviderAliasesStructure() { ProviderAlias = "unexpected" }];

        // Action
        ProviderModel sut = source;

        Assert.That(sut.TradingName, Is.EqualTo(expectedTradingName));
    }

    [Test, AutoData]
    public void WhenConvertingFromProvider_MapsContactDetailsFromPrimaryContact(MatchingProviderRecords source, ProviderContactStructure contact, ContactPersonalDetailsStructure expectedProviderContact, string expectedEmail, string expectedUrl, string expectedPhone, ProviderContactStructure unexpectedContact)
    {
        source.UnitedKingdomProviderReferenceNumber = "10012001";
        contact.ContactType = "L";
        contact.ContactPersonalDetails = expectedProviderContact;
        source.ProviderContacts = [unexpectedContact, contact];

        // Action
        ProviderModel sut = source;

        Assert.Multiple(() =>
        {
            Assert.That(sut.ContactDetails.Title, Is.EqualTo(expectedProviderContact.PersonNameTitle));
            Assert.That(sut.ContactDetails.FirstName, Is.EqualTo(expectedProviderContact.PersonGivenName));
            Assert.That(sut.ContactDetails.LastName, Is.EqualTo(expectedProviderContact.PersonFamilyName));
            Assert.That(sut.ContactDetails.Email, Is.EqualTo(contact.ContactEmail));
            Assert.That(sut.ContactDetails.Website, Is.EqualTo(contact.ContactWebsiteAddress));
            Assert.That(sut.ContactDetails.Telephone, Is.EqualTo(contact.ContactTelephone1));
        });
    }

    [Test, AutoData]
    public void WhenConvertingFromProvider_MapsLegalAddressFromLegalAddress(MatchingProviderRecords source, ProviderContactStructure contact, ProviderContactAddress expectedAddress)
    {
        source.UnitedKingdomProviderReferenceNumber = "10012001";
        contact.ContactType = "L";
        contact.ContactAddress = expectedAddress;
        source.ProviderContacts = [contact];

        // Action
        ProviderModel sut = source;

        Assert.Multiple(() =>
        {
            Assert.That(sut.LegalAddress.Address1, Is.EqualTo(expectedAddress.Address1));
            Assert.That(sut.LegalAddress.Address2, Is.EqualTo(expectedAddress.Address2));
            Assert.That(sut.LegalAddress.Address3, Is.EqualTo(expectedAddress.Address3));
            Assert.That(sut.LegalAddress.Address4, Is.EqualTo(expectedAddress.Address4));
            Assert.That(sut.LegalAddress.Town, Is.EqualTo(expectedAddress.Town));
            Assert.That(sut.LegalAddress.Postcode, Is.EqualTo(expectedAddress.PostCode));
        });
    }

    [Test, AutoData]
    public void WhenConvertingFromProvider_MapsVerificationDetails(MatchingProviderRecords source, IEnumerable<VerificationDetailsStructure> expectedVerificationDetails)
    {
        source.UnitedKingdomProviderReferenceNumber = "10012001";
        source.ProviderContacts = [new ProviderContactStructure() { ContactType = "L" }];
        source.VerificationDetails = [.. expectedVerificationDetails];

        // Action
        ProviderModel sut = source;

        sut.VerificationDetails.Should().BeEquivalentTo(expectedVerificationDetails, opt => opt.WithMapping<VerificationInfo>(s => s.VerificationId, t => t.VerificationID));
    }
}
