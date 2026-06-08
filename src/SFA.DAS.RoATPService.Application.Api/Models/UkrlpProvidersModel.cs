using System.Collections.Generic;
using System.Linq;
using SFA.DAS.RoATPService.Ukrlp.Client;

namespace SFA.DAS.RoATPService.Application.Api.Models;

public record UkrlpProvidersModel(IEnumerable<ProviderModel> Providers);

public class ProviderModel
{
    public int Ukprn { get; set; }
    public string LegalName { get; set; }
    public string TradingName { get; set; }
    public ProviderContactModel ContactDetails { get; set; }
    public Address Address { get; set; }
    public IEnumerable<VerificationInfo> VerificationDetails { get; set; }

    public static implicit operator ProviderModel(Provider provider)
    {
        return new ProviderModel
        {
            Ukprn = int.Parse(provider.UKPRN),
            LegalName = provider.ProviderName,
            TradingName = provider.ProviderAliases.FirstOrDefault()?.Name,
            ContactDetails = new ProviderContactModel(
                provider.PrimaryContact?.ContactPersonalDetails?.PersonNameTitle,
                provider.PrimaryContact?.ContactPersonalDetails?.PersonGivenName,
                provider.PrimaryContact?.ContactPersonalDetails?.PersonFamilyName,
                provider.PrimaryContact?.ContactEmail,
                provider.PrimaryContact?.ContactTelephone1 ?? provider.PrimaryContact?.ContactTelephone2,
                provider.PrimaryContact?.Url),
            Address = provider.LegalAddress,
            VerificationDetails = provider.VerificationDetails
        };
    }
}

public record ProviderContactModel(string Title, string FirstName, string LastName, string Email, string Telephone, string Website);


