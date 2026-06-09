namespace SFA.DAS.RoATPService.Application.Api.Extensions;

public static class StringExtensions
{
    public static string NullIfEmpty(this string value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
