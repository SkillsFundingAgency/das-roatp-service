using System;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Models;

namespace SFA.DAS.RoATPService.Api.UnitTests.Models;

public class ProviderDetailsTests
{
    [Test, AutoData]
    public void ProviderDetails_ImplicitConversion_From_MatchingProviderRecords_Works(Ukrlp.Client.Provider expected)
    {
        ProviderDetails sut = expected;
        Assert.Multiple(() =>
        {
            Assert.That(sut.UKPRN, Is.EqualTo(expected.UKPRN));
            Assert.That(sut.ProviderName, Is.EqualTo(expected.ProviderName));
            Assert.That(sut.ProviderStatus, Is.EqualTo(expected.ProviderStatus));
            Assert.That(sut.VerificationDate, Is.EqualTo(expected.VerificationDate));
            Assert.That(sut.ContactDetails?.Count(), Is.EqualTo(1), "There is only one PrimaryContact in the MatchingProviderRecords, and this is mapped to ContactDetails");
            Assert.That(sut.ProviderAliases?.Count(), Is.EqualTo(expected.ProviderAliases?.Count()));
            Assert.That(sut.VerificationDetails?.Count(), Is.EqualTo(expected.VerificationDetails?.Count()));
        });
    }

    [Test, AutoData]
    public void ProviderContact_ImplicitConversion_From_ProviderContactStructure_Works(Ukrlp.Client.Provider expectedProviderContact)
    {
        ProviderContact sut = expectedProviderContact;
        Assert.Multiple(() =>
        {
            Assert.That(sut.ContactType, Is.EqualTo("L"), "Only legal contact is available now");
            sut.ContactAddress.Should().BeEquivalentTo(expectedProviderContact.LegalAddress, options => options.ExcludingMissingMembers());
            sut.ContactPersonalDetails.Should().BeEquivalentTo(expectedProviderContact.PrimaryContact.ContactPersonalDetails);
            Assert.That(sut.ContactRole, Is.EqualTo(expectedProviderContact.PrimaryContact.ContactRole));
            Assert.That(sut.ContactTelephone1, Is.EqualTo(expectedProviderContact.PrimaryContact.ContactTelephone1));
            Assert.That(sut.ContactTelephone2, Is.EqualTo(expectedProviderContact.PrimaryContact.ContactTelephone2));
            Assert.That(sut.ContactWebsiteAddress, Is.EqualTo(expectedProviderContact.PrimaryContact.Url));
            Assert.That(sut.ContactEmail, Is.EqualTo(expectedProviderContact.PrimaryContact.ContactEmail));
            Assert.That(sut.LastUpdated, Is.EqualTo(expectedProviderContact.PrimaryContact.LastUpdated));
        });
    }

    [Test, AutoData]
    public void ProviderAlias_ImplicitConversion_From_ProviderAliasStructure_Works(Ukrlp.Client.ProviderAlias expectedProviderAlias)
    {
        ProviderAlias sut = expectedProviderAlias;
        Assert.Multiple(() =>
        {
            Assert.That(sut.Alias, Is.EqualTo(expectedProviderAlias.Name));
        });
    }

    [Test, AutoData]
    public void VerificationDetails_ImplicitConversion_From_VerificationDetailsStructure_Works(Ukrlp.Client.VerificationInfo expectedVerificationDetails)
    {
        VerificationDetails sut = expectedVerificationDetails;
        Assert.Multiple(() =>
        {
            Assert.That(sut.VerificationAuthority, Is.EqualTo(expectedVerificationDetails.VerificationAuthority));
            Assert.That(sut.VerificationId, Is.EqualTo(expectedVerificationDetails.VerificationID));
            Assert.That(sut.PrimaryVerificationSource, Is.EqualTo(expectedVerificationDetails.PrimaryVerificationSource));
        });
    }

    [Test, AutoData]
    public void ContactAddress_ImplicitConversion_From_ContactAddressStructure_Works(Ukrlp.Client.Address expectedContactAddress)
    {
        ContactAddress sut = expectedContactAddress;
        Assert.Multiple(() =>
        {
            Assert.That(sut.Address1, Is.EqualTo(expectedContactAddress.Address1));
            Assert.That(sut.Address2, Is.EqualTo(expectedContactAddress.Address2));
            Assert.That(sut.Address3, Is.EqualTo(expectedContactAddress.Address3));
            Assert.That(sut.Address4, Is.EqualTo(expectedContactAddress.Address4));
            Assert.That(sut.Town, Is.EqualTo(expectedContactAddress.Town));
            Assert.That(sut.PostCode, Is.EqualTo(expectedContactAddress.PostCode));
        });
    }

    [Test, AutoData]
    public void ContactPersonalDetails_ImplicitConversion_From_ContactPersonalDetailsStructure_Works(Ukrlp.Client.ContactPerson expectedContactPersonalDetails)
    {
        ContactPersonalDetails sut = expectedContactPersonalDetails;
        Assert.Multiple(() =>
        {
            Assert.That(sut.PersonNameTitle, Is.EqualTo(expectedContactPersonalDetails.PersonNameTitle));
            Assert.That(sut.PersonGivenName, Is.EqualTo(expectedContactPersonalDetails.PersonGivenName));
            Assert.That(sut.PersonFamilyName, Is.EqualTo(expectedContactPersonalDetails.PersonFamilyName));
        });
    }
}
