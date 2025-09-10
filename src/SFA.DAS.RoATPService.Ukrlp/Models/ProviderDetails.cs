using System;

namespace SFA.DAS.RoATPService.Ukrlp.Models;

public class ProviderDetails
{
    public string UKPRN { get; set; }
    public string ProviderName { get; set; }
    public string ProviderStatus { get; set; }
    public DateTime? VerificationDate { get; set; }
    public ProviderContact[] ContactDetails { get; set; } = [];
    public ProviderAlias[] ProviderAliases { get; set; } = [];
    public VerificationDetails[] VerificationDetails { get; set; } = [];

    public static implicit operator ProviderDetails(ProviderRecordStructure source)
    {
        return new()
        {
            UKPRN = source.UnitedKingdomProviderReferenceNumber,
            ProviderName = source.ProviderName,
            ProviderStatus = source.ProviderStatus,
            VerificationDate = source.ProviderVerificationDate,
            ContactDetails = source.ProviderContact != null ? Array.ConvertAll(source.ProviderContact, c => (ProviderContact)c) : [],
            ProviderAliases = source.ProviderAliases != null ? Array.ConvertAll(source.ProviderAliases, a => (ProviderAlias)a) : [],
            VerificationDetails = source.VerificationDetails != null ? Array.ConvertAll(source.VerificationDetails, v => (VerificationDetails)v) : []
        };
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
        return new()
        {
            ContactType = source.ContactType,
            ContactAddress = source.ContactAddress != null ? (ContactAddress)source.ContactAddress : null,
            ContactPersonalDetails = source.ContactPersonalDetails != null ? (ContactPersonalDetails)source.ContactPersonalDetails : null,
            ContactRole = source.ContactRole,
            ContactTelephone1 = source.ContactTelephone1,
            ContactTelephone2 = source.ContactTelephone2,
            ContactWebsiteAddress = source.ContactWebsiteAddress,
            ContactEmail = source.ContactEmail,
            LastUpdated = source.LastUpdated
        };
    }
}

public class ProviderAlias
{
    public string Alias { get; set; }
    public DateTime? LastUpdated { get; set; }
    public static implicit operator ProviderAlias(ProviderAliasesStructure source)
    {
        return new()
        {
            Alias = source.ProviderAlias,
            LastUpdated = source.LastUpdated != null
                ? DateTime.Parse(source.LastUpdated.ToString(), System.Globalization.CultureInfo.InvariantCulture)
                : null
        };
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

    public static implicit operator ContactAddress(AddressStructure source)
    {
        return new()
        {
            Address1 = source.Address1,
            Address2 = source.Address2,
            Address3 = source.Address3,
            Address4 = source.Address4,
            Town = source.Town,
            PostCode = source.PostCode
        };
    }
}

public class ContactPersonalDetails
{
    public string PersonNameTitle { get; set; }
    public string PersonGivenName { get; set; }
    public string PersonFamilyName { get; set; }
    public string PersonNameSuffix { get; set; }
    public static implicit operator ContactPersonalDetails(PersonNameStructure source)
    {
        return new()
        {
            PersonNameTitle = source.PersonNameTitle != null && source.PersonNameTitle.Length > 0 ? source.PersonNameTitle[0] : null,
            PersonGivenName = source.PersonGivenName != null && source.PersonGivenName.Length > 0 ? source.PersonGivenName[0] : null,
            PersonFamilyName = source.PersonFamilyName,
            PersonNameSuffix = source.PersonNameSuffix != null && source.PersonNameSuffix.Length > 0 ? source.PersonNameSuffix[0] : null
        };
    }
}

public class VerificationDetails
{
    public string VerificationAuthority { get; set; }
    public string VerificationId { get; set; }
    public bool PrimaryVerificationSource { get; set; }
    public static implicit operator VerificationDetails(VerificationDetailsResponseStructure source)
    {
        return new()
        {
            VerificationAuthority = source.VerificationAuthority,
            VerificationId = source.VerificationID,
            PrimaryVerificationSource = source.PrimaryVerificationSource != null ? bool.Parse(source.PrimaryVerificationSource) : default
        };
    }
}
