using System;
using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Ukrlp.Client;

public record UkrlpResponse(IEnumerable<Provider> MatchingProviderRecords);

public class Provider
{
    public string UKPRN { get; set; }
    public string ProviderName { get; set; }
    public string ProviderStatus { get; set; }
    public DateTime? VerificationDate { get; set; }
    public Address LegalAddress { get; set; }
    public Address TradingAddress { get; set; }
    public ProviderContact PrimaryContact { get; set; }
    public IEnumerable<ProviderAlias> ProviderAliases { get; set; } = [];
    public IEnumerable<VerificationInfo> VerificationDetails { get; set; } = [];
}

public record Address(string Address1, string Address2, string Address3, string Address4, string Town, string County, string PostCode);

public record ProviderContact(
    ContactPerson ContactPersonalDetails,
    string ContactRole,
    string ContactTelephone1,
    string ContactTelephone2,
    string ContactEmail,
    string Url,
    DateTime? LastUpdated);

public record ContactPerson(string PersonNameTitle, string PersonGivenName, string PersonFamilyName);

public record ProviderAlias(string Name);

public class VerificationInfo
{
    public string VerificationAuthority { get; set; }
    public string VerificationID { get; set; }
    public bool PrimaryVerificationSource { get; set; }
}
