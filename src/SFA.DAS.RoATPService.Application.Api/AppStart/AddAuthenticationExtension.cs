using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.RoATPService.Application.Api.Configuration;
using SFA.DAS.RoATPService.Application.Api.Infrastructure;

namespace SFA.DAS.RoATPService.Application.AppStart;

public static class AddAuthenticationExtension
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization(o =>
        {
            o.AddPolicy(Roles.RoATPServiceInternalAPI, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(Roles.RoATPServiceInternalAPI);
            });
        });

        var apiAuthentication = configuration.GetSection("ApiAuthentication").Get<ApiAuthentication>();
        services
            .AddAuthentication(o => { o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
            .AddJwtBearer(o =>
            {
                o.Authority = $"https://login.microsoftonline.com/{apiAuthentication.TenantId}";
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    ValidAudiences = apiAuthentication.Audience.Split(',')
                };
                o.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = context => { return Task.FromResult(0); }
                };
            });

        return services;
    }
}
