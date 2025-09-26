namespace SFA.DAS.RoATPService.Settings;

public class WebConfiguration
{
    public ApiAuthentication ApiAuthentication { get; set; }
    public string SqlConnectionString { get; set; }

    public string SessionRedisConnectionString { get; set; }

    public UkrlpApiAuthentication UkrlpApiAuthentication { get; set; }
}
