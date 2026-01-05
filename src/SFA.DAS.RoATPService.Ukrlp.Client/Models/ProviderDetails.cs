using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.RoATPService.Ukrlp.Client.Models;

public class ProviderDetails
{
    public string UKPRN { get; set; }
    public string ProviderName { get; set; }
    public string ProviderStatus { get; set; }
    public IEnumerable<ProviderContact> ContactDetails { get; set; }
    public DateTime? VerificationDate { get; set; }
    public IEnumerable<ProviderAlias> ProviderAliases { get; set; }
    public IEnumerable<VerificationDetails> VerificationDetails { get; set; }

    public static implicit operator ProviderDetails(MatchingProviderRecords source)
    {
        ProviderDetails target = new()
        {
            UKPRN = source.UnitedKingdomProviderReferenceNumber,
            ProviderName = source.ProviderName,
            ProviderStatus = source.ProviderStatus,
            ContactDetails = source.ProviderContacts.Select(c => (ProviderContact)c),
            VerificationDate = source.ProviderVerificationDate,
            ProviderAliases = source.ProviderAliases.Select(a => (ProviderAlias)a),
            VerificationDetails = source.VerificationDetails.Select(v => (VerificationDetails)v)
        };

        return target;
    }

}

public class ProviderContact
{
    public string ContactType { get; set; }
    public ContactAddress ContactAddress { get; set; }
    public ContactPersonalDetails ContactPersonalDetails { get; set; }
    public string ContactRole { get; set; }
    public string ContactTelephone1 { get; set; }
    public string ContactTelephone2 { get; set; }
    public string ContactWebsiteAddress { get; set; }
    public string ContactEmail { get; set; }
    public DateTime? LastUpdated { get; set; }

    public static implicit operator ProviderContact(ProviderContactStructure source)
    {
        ProviderContact target = new()
        {
            ContactType = source.ContactType,
            ContactAddress = source.ContactAddress,
            ContactPersonalDetails = source.ContactPersonalDetails,
            ContactRole = source.ContactRole,
            ContactTelephone1 = source.ContactTelephone1,
            ContactTelephone2 = source.ContactTelephone2,
            ContactWebsiteAddress = source.ContactWebsiteAddress,
            ContactEmail = source.ContactEmail,
            LastUpdated = source.LastUpdated
        };
        return target;
    }
}

public class ProviderAlias
{
    public string Alias { get; set; }
    public DateTime? LastUpdated { get; set; }
    public static implicit operator ProviderAlias(ProviderAliasesStructure source)
    {
        ProviderAlias target = new()
        {
            Alias = source.ProviderAlias,
            LastUpdated = source.LastUpdated
        };
        return target;
    }
}

public class ContactAddress
{
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string Address3 { get; set; }
    public string Address4 { get; set; }
    public string Town { get; set; }
    public string PostCode { get; set; }
    public static implicit operator ContactAddress(ProviderContactAddress source)
    {
        ContactAddress target = new()
        {
            Address1 = source.Address1,
            Address2 = source.Address2,
            Address3 = source.Address3,
            Address4 = source.Address4,
            Town = source.Town,
            PostCode = source.PostCode
        };
        return target;
    }
}

public class ContactPersonalDetails
{
    public string PersonNameTitle { get; set; }
    public string PersonGivenName { get; set; }
    public string PersonFamilyName { get; set; }
    public string PersonNameSuffix { get; set; }
    public static implicit operator ContactPersonalDetails(ContactPersonalDetailsStructure source)
    {
        ContactPersonalDetails target = new()
        {
            PersonNameTitle = source.PersonNameTitle,
            PersonGivenName = source.PersonGivenName,
            PersonFamilyName = source.PersonFamilyName,
            PersonNameSuffix = source.PersonNameSuffix
        };
        return target;
    }
}

public class VerificationDetails
{
    public string VerificationAuthority { get; set; }
    public string VerificationId { get; set; }
    public bool PrimaryVerificationSource { get; set; }
    public static implicit operator VerificationDetails(VerificationDetailsStructure source)
    {
        VerificationDetails target = new()
        {
            VerificationAuthority = source.VerificationAuthority,
            VerificationId = source.VerificationId,
            PrimaryVerificationSource = source.PrimaryVerificationSource
        };
        return target;
    }
}
