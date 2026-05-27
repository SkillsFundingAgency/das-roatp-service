using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.RoATPService.Ukrlp.Client;

[ExcludeFromCodeCoverage]
public class UkrlpApiAuthentication
{
    public string ApiBaseAddress { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Scope { get; set; }
    public string SubscriptionKey { get; set; }
    public string TokenEndpoint { get; set; }
}
