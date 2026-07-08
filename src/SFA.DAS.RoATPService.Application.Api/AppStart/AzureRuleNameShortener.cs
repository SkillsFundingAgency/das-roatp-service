using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace SFA.DAS.RoATPService.Application.Api.AppStart;

[ExcludeFromCodeCoverage]
public static partial class ConfigureNServiceBusExtension
{
    public static class AzureRuleNameShortener
    {
        private const int AzureServiceBusRuleNameMaxLength = 50;

        public static string Shorten(Type arg)
        {
            var ruleName = arg.FullName;
            if (ruleName!.Length <= AzureServiceBusRuleNameMaxLength)
            {
                return ruleName;
            }

            var bytes = Encoding.UTF8.GetBytes(ruleName);
            var hash = SHA256.HashData(bytes);
            var shortenedRuleName = Convert.ToHexString(hash)[..AzureServiceBusRuleNameMaxLength];

            return shortenedRuleName;
        }
    }
}
