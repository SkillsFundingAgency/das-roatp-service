using System.Collections.Generic;
using System.Linq;
using SFA.DAS.RoATPService.Application.Api.Extensions;
using SFA.DAS.RoATPService.Ukrlp.Client;

namespace SFA.DAS.RoATPService.Application.Api.Models;

public record UkrlpProvidersModel(IEnumerable<ProviderModel> Providers);

public class ProviderModel
{
    public int Ukprn { get; set; }
    public string LegalName { get; set; }
    public string TradingName { get; set; }
    public ProviderContactModel ContactDetails { get; set; }
    public Address LegalAddress { get; set; }
    public IEnumerable<VerificationInfo> VerificationDetails { get; set; }

    public static implicit operator ProviderModel(Provider provider)
    {
        return new ProviderModel
        {
            Ukprn = int.Parse(provider.UKPRN),
            LegalName = provider.ProviderName.Trim(),
            TradingName = provider.ProviderAliases.FirstOrDefault()?.Name.NullIfEmpty(),
            ContactDetails = new ProviderContactModel(
                provider.PrimaryContact?.ContactPersonalDetails?.PersonNameTitle.NullIfEmpty(),
                provider.PrimaryContact?.ContactPersonalDetails?.PersonGivenName.NullIfEmpty(),
                provider.PrimaryContact?.ContactPersonalDetails?.PersonFamilyName.NullIfEmpty(),
                provider.PrimaryContact?.ContactEmail.NullIfEmpty(),
                provider.PrimaryContact?.ContactTelephone1 ?? provider.PrimaryContact?.ContactTelephone2.NullIfEmpty(),
                provider.PrimaryContact?.Url.NullIfEmpty()),
            LegalAddress = new(
                provider.LegalAddress?.Address1.NullIfEmpty(),
                provider.LegalAddress?.Address2.NullIfEmpty(),
                provider.LegalAddress?.Address3.NullIfEmpty(),
                provider.LegalAddress?.Address4.NullIfEmpty(),
                provider.LegalAddress?.Town.NullIfEmpty(),
                provider.LegalAddress?.County.NullIfEmpty(),
                provider.LegalAddress?.Postcode.NullIfEmpty()),
            VerificationDetails = provider.VerificationDetails.Select(v => new VerificationInfo
            {
                VerificationAuthority = v.VerificationAuthority.NullIfEmpty(),
                VerificationID = v.VerificationID.NullIfEmpty(),
                PrimaryVerificationSource = v.PrimaryVerificationSource
            })
        };
    }
}

public record ProviderContactModel(string Title, string FirstName, string LastName, string Email, string Telephone, string Website);


