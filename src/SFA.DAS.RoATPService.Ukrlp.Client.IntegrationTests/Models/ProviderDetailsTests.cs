using System;
using System.Linq;
using AutoFixture.NUnit3;
using NUnit.Framework;
using SFA.DAS.RoATPService.Ukrlp.Client.Models;

namespace SFA.DAS.RoATPService.Ukrlp.Client.IntegrationTests.Models;

public class ProviderDetailsTests
{
    [Test, AutoData]
    public void ProviderDetails_ImplicitConversion_From_MatchingProviderRecords_Works(MatchingProviderRecords expectedProviderDetails)
    {
        ProviderDetails sut = expectedProviderDetails;
        Assert.Multiple(() =>
        {
            Assert.That(sut.ProviderName, Is.EqualTo(expectedProviderDetails.ProviderName));
            Assert.That(sut.ProviderStatus, Is.EqualTo(expectedProviderDetails.ProviderStatus));
            Assert.That(sut.VerificationDate, Is.EqualTo(expectedProviderDetails.ProviderVerificationDate));
            Assert.That(sut.ContactDetails?.Count(), Is.EqualTo(expectedProviderDetails.ProviderContacts?.Count));
            Assert.That(sut.ProviderAliases?.Count(), Is.EqualTo(expectedProviderDetails.ProviderAliases?.Count));
            Assert.That(sut.VerificationDetails?.Count(), Is.EqualTo(expectedProviderDetails.VerificationDetails?.Count));
        });
    }

    [Test, AutoData]
    public void ProviderContact_ImplicitConversion_From_ProviderContactStructure_Works(ProviderContactStructure expectedProviderContact)
    {
        ProviderContact sut = expectedProviderContact;
        Assert.Multiple(() =>
        {
            Assert.That(sut.ContactType, Is.EqualTo(expectedProviderContact.ContactType));
            Assert.That(sut.ContactRole, Is.EqualTo(expectedProviderContact.ContactRole));
            Assert.That(sut.ContactTelephone1, Is.EqualTo(expectedProviderContact.ContactTelephone1));
            Assert.That(sut.ContactTelephone2, Is.EqualTo(expectedProviderContact.ContactTelephone2));
            Assert.That(sut.ContactWebsiteAddress, Is.EqualTo(expectedProviderContact.ContactWebsiteAddress));
            Assert.That(sut.ContactEmail, Is.EqualTo(expectedProviderContact.ContactEmail));
            Assert.That(sut.LastUpdated, Is.EqualTo(expectedProviderContact.LastUpdated));
        });
    }

    [Test, AutoData]
    public void ProviderAlias_ImplicitConversion_From_ProviderAliasStructure_Works(ProviderAliasesStructure expectedProviderAlias)
    {
        ProviderAlias sut = expectedProviderAlias;
        Assert.Multiple(() =>
        {
            Assert.That(sut.Alias, Is.EqualTo(expectedProviderAlias.ProviderAlias));
            Assert.That(sut.LastUpdated, Is.EqualTo(expectedProviderAlias.LastUpdated));
        });
    }

    [Test, AutoData]
    public void VerificationDetails_ImplicitConversion_From_VerificationDetailsStructure_Works(VerificationDetailsStructure expectedVerificationDetails)
    {
        VerificationDetails sut = expectedVerificationDetails;
        Assert.Multiple(() =>
        {
            Assert.That(sut.VerificationAuthority, Is.EqualTo(expectedVerificationDetails.VerificationAuthority));
            Assert.That(sut.VerificationId, Is.EqualTo(expectedVerificationDetails.VerificationId));
            Assert.That(sut.PrimaryVerificationSource, Is.EqualTo(expectedVerificationDetails.PrimaryVerificationSource));
        });
    }

    [Test, AutoData]
    public void ContactAddress_ImplicitConversion_From_ContactAddressStructure_Works(ProviderContactAddress expectedContactAddress)
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
    public void ContactPersonalDetails_ImplicitConversion_From_ContactPersonalDetailsStructure_Works(ContactPersonalDetailsStructure expectedContactPersonalDetails)
    {
        ContactPersonalDetails sut = expectedContactPersonalDetails;
        Assert.Multiple(() =>
        {
            Assert.That(sut.PersonNameTitle, Is.EqualTo(expectedContactPersonalDetails.PersonNameTitle));
            Assert.That(sut.PersonGivenName, Is.EqualTo(expectedContactPersonalDetails.PersonGivenName));
            Assert.That(sut.PersonFamilyName, Is.EqualTo(expectedContactPersonalDetails.PersonFamilyName));
            Assert.That(sut.PersonNameSuffix, Is.EqualTo(expectedContactPersonalDetails.PersonNameSuffix));
        });
    }
}
