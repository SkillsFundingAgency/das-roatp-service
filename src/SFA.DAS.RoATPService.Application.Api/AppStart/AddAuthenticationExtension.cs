using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.RoATPService.Application.Api.Extensions;

namespace SFA.DAS.RoATPService.Application.AppStart;

public static class AddAuthenticationExtension
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        if (!configuration.IsLocalEnvironment())
        {
            var azureAdConfiguration = configuration
                .GetSection("AzureAd")
                .Get<AzureActiveDirectoryConfiguration>()!;

            var policies = new Dictionary<string, string>
            {
                {PolicyNames.Default, PolicyNames.Default},
            };

            services.AddAuthentication(azureAdConfiguration, policies);
        }

        return services;
    }
}
