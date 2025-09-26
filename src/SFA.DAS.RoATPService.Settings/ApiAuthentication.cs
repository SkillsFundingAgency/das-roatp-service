namespace SFA.DAS.RoATPService.Settings;

public class ApiAuthentication : IApiAuthentication
{
    public string ClientId { get; set; }

    public string Instance { get; set; }

    public string TenantId { get; set; }

    public string Audience { get; set; }
}