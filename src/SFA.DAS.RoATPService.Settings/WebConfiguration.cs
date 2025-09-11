using System.Text.Json.Serialization;

namespace SFA.DAS.RoATPService.Settings;

public class WebConfiguration
{
    [JsonRequired]
    public string SqlConnectionString { get; set; }

    public string SessionRedisConnectionString { get; set; }

    public UkrlpApiAuthentication UkrlpApiAuthentication { get; set; }
}
