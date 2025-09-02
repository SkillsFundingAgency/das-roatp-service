using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.RoATPService.Application.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class ConfigurationExtensions
{
    public static bool IsLocalEnvironment(this IConfiguration configuration)
    {
        var environmentName = configuration["EnvironmentName"]!;
        return environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
               environmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
    }
}
