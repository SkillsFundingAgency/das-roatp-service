namespace SFA.DAS.RoATPService.Ukrlp.Client;

public class UkrlpApiAuthentication
{
    public string ApiBaseAddress { get; set; }
    public string StakeholderId { get; set; }
    public string QueryId { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Scope { get; set; }
    public string SubscriptionKey { get; set; }
    public string TokenEndpoint { get; set; }
}
