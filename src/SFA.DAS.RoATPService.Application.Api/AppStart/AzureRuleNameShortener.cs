using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

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
            var bytes = System.Text.Encoding.Default.GetBytes(ruleName);
            var hash = MD5.HashData(bytes);
            var shortenedRuleName = new Guid(hash).ToString();

            return shortenedRuleName;
        }
    }
}
