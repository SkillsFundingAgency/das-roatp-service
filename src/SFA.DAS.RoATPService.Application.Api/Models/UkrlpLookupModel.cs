using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.RoATPService.Application.Api.Extensions;

namespace SFA.DAS.RoATPService.Application.Api.Models;

/// <summary>
/// Old model used for the UkrlpLookup endpoint which is being deprecated.
/// </summary>
/// <param name="Success"></param>
/// <param name="Results"></param>
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
        => new()
        {
            ContactType = "L",
            ContactAddress = source.LegalAddress,
            ContactPersonalDetails = source.PrimaryContact?.ContactPersonalDetails,
            ContactRole = source.PrimaryContact?.ContactRole.NullIfEmpty(),
            ContactTelephone1 = source.PrimaryContact?.ContactTelephone1.NullIfEmpty(),
            ContactTelephone2 = source.PrimaryContact?.ContactTelephone2.NullIfEmpty(),
            ContactWebsiteAddress = source.PrimaryContact?.Url.NullIfEmpty(),
            ContactEmail = source.PrimaryContact?.ContactEmail.NullIfEmpty(),
            LastUpdated = source.PrimaryContact?.LastUpdated
        };
}

public class ProviderAlias
{
    public string Alias { get; set; }
    [Obsolete("This information is not included in the new Ukrlp Api response")]
    public DateTime? LastUpdated => null;
    public static implicit operator ProviderAlias(Ukrlp.Client.ProviderAlias source)
        => new()
        {
            Alias = source.Name
        };
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
        => new()
        {
            Address1 = source.Address1.NullIfEmpty(),
            Address2 = source.Address2.NullIfEmpty(),
            Address3 = source.Address3.NullIfEmpty(),
            Address4 = source.Address4.NullIfEmpty(),
            Town = source.Town.NullIfEmpty(),
            PostCode = source.Postcode.NullIfEmpty()
        };
}

public class ContactPersonalDetails
{
    public string PersonNameTitle { get; set; }
    public string PersonGivenName { get; set; }
    public string PersonFamilyName { get; set; }
    [Obsolete("This information is not included in the new Ukrlp Api response")]
    public string PersonNameSuffix => null;

    public static implicit operator ContactPersonalDetails(Ukrlp.Client.ContactPerson source)
        => new()
        {
            PersonNameTitle = source.PersonNameTitle.NullIfEmpty(),
            PersonGivenName = source.PersonGivenName.NullIfEmpty(),
            PersonFamilyName = source.PersonFamilyName.NullIfEmpty()
        };
}

public class VerificationDetails
{
    public string VerificationAuthority { get; set; }
    public string VerificationId { get; set; }
    public bool PrimaryVerificationSource { get; set; }

    public static implicit operator VerificationDetails(Ukrlp.Client.VerificationInfo source)
        => new()
        {
            VerificationAuthority = source.VerificationAuthority.NullIfEmpty(),
            VerificationId = source.VerificationID.NullIfEmpty(),
            PrimaryVerificationSource = source?.PrimaryVerificationSource ?? false
        };
}
