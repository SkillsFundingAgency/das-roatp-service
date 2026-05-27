using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.RoATPService.Application.Api.Models;

public record UkrlpLookupModel(bool Success, IEnumerable<ProviderDetails> Results);

public class ProviderDetails
{
    public string UKPRN { get; set; }
    public string ProviderName { get; set; }
    public string ProviderStatus { get; set; }
    public IEnumerable<ProviderContact> ContactDetails { get; set; } = [];
    public DateTime? VerificationDate { get; set; }
    public IEnumerable<ProviderAlias> ProviderAliases { get; set; } = [];
    public IEnumerable<VerificationDetails> VerificationDetails { get; set; } = [];

    public static implicit operator ProviderDetails(Ukrlp.Client.Provider source)
    {
        ProviderDetails target = new()
        {
            UKPRN = source.UKPRN,
            ProviderName = source.ProviderName,
            ProviderStatus = source.ProviderStatus,
            VerificationDate = source.VerificationDate,
            ContactDetails = [(ProviderContact)source],
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

    public static implicit operator ProviderContact(Ukrlp.Client.Provider source)
    {
        ProviderContact target = new()
        {
            ContactType = "L",
            ContactAddress = source.LegalAddress,
            ContactPersonalDetails = source.PrimaryContact.ContactPersonalDetails,
            ContactRole = string.IsNullOrWhiteSpace(source.PrimaryContact.ContactRole) ? null : source.PrimaryContact.ContactRole,
            ContactTelephone1 = string.IsNullOrWhiteSpace(source.PrimaryContact.ContactTelephone1) ? null : source.PrimaryContact.ContactTelephone1,
            ContactTelephone2 = source.PrimaryContact.ContactTelephone2,
            ContactWebsiteAddress = source.PrimaryContact.Url,
            ContactEmail = source.PrimaryContact.ContactEmail,
            LastUpdated = source.PrimaryContact.LastUpdated
        };
        return target;
    }
}

public class ProviderAlias
{
    public string Alias { get; set; }
    [Obsolete("This information is not included in the new Ukrlp Api response")]
    public DateTime? LastUpdated => null;
    public static implicit operator ProviderAlias(Ukrlp.Client.ProviderAlias source)
    {
        ProviderAlias target = new()
        {
            Alias = source.Name
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
    public static implicit operator ContactAddress(Ukrlp.Client.Address source)
    {
        ContactAddress target = new()
        {
            Address1 = string.IsNullOrWhiteSpace(source.Address1) ? null : source.Address1,
            Address2 = string.IsNullOrWhiteSpace(source.Address2) ? null : source.Address2,
            Address3 = string.IsNullOrWhiteSpace(source.Address3) ? null : source.Address3,
            Address4 = string.IsNullOrWhiteSpace(source.Address4) ? null : source.Address4,
            Town = string.IsNullOrWhiteSpace(source.Town) ? null : source.Town,
            PostCode = string.IsNullOrWhiteSpace(source.PostCode) ? null : source.PostCode
        };
        return target;
    }
}

public class ContactPersonalDetails
{
    public string PersonNameTitle { get; set; }
    public string PersonGivenName { get; set; }
    public string PersonFamilyName { get; set; }
    [Obsolete("This information is not included in the new Ukrlp Api response")]
    public string PersonNameSuffix => null;

    public static implicit operator ContactPersonalDetails(Ukrlp.Client.ContactPerson source)
    {
        ContactPersonalDetails target = new()
        {
            PersonNameTitle = string.IsNullOrWhiteSpace(source.PersonNameTitle) ? null : source.PersonNameTitle,
            PersonGivenName = string.IsNullOrWhiteSpace(source.PersonGivenName) ? null : source.PersonGivenName,
            PersonFamilyName = string.IsNullOrWhiteSpace(source.PersonFamilyName) ? null : source.PersonFamilyName
        };
        return target;
    }
}

public class VerificationDetails
{
    public string VerificationAuthority { get; set; }
    public string VerificationId { get; set; }
    public bool PrimaryVerificationSource { get; set; }

    public static implicit operator VerificationDetails(Ukrlp.Client.VerificationInfo source)
    {
        VerificationDetails target = new()
        {
            VerificationAuthority = string.IsNullOrWhiteSpace(source.VerificationAuthority) ? null : source.VerificationAuthority,
            VerificationId = string.IsNullOrWhiteSpace(source.VerificationId) ? null : source.VerificationId,
            //PrimaryVerificationSource = source.PrimaryVerificationSource
        };
        return target;
    }
}
