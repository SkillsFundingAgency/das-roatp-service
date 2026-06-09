using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Extensions;

namespace SFA.DAS.RoATPService.Api.UnitTests.Extensions;

public class StringExtensionsTests
{
    [TestCase(null, null)]
    [TestCase("", null)]
    [TestCase("  ", null)]
    [TestCase(" trimmed ", "trimmed")]
    public void NullIfEmpty_WhenStringIsNull_ReturnsNull(string input, string expected)
    {
        string actual = input.NullIfEmpty();
        Assert.That(actual, Is.EqualTo(expected));
    }
}
