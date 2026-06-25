using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.RoATPService.Ukrlp.SoapClient;

[ExcludeFromCodeCoverage]
public class UkrlpSoapApiAuthentication
{
    public string ApiBaseAddress { get; set; }
    public string StakeholderId { get; set; }
    public string QueryId { get; set; }
}
