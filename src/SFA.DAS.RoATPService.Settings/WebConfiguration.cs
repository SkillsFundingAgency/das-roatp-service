using System.Text.Json.Serialization;
using SFA.DAS.RoATPService.Ukrlp;

namespace SFA.DAS.RoATPService.Settings;

public class WebConfiguration
{
    [JsonRequired]
    public string SqlConnectionString { get; set; }

    public string SessionRedisConnectionString { get; set; }

    public UkrlpApiConfiguration UkrlpApiAuthentication { get; set; }
}
