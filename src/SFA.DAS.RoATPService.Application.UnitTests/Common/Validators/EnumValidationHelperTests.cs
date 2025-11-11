using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Common.Validators;
using SFA.DAS.RoATPService.Domain.Common;

namespace SFA.DAS.RoATPService.Application.UnitTests.Common.Validators;
public class EnumValidationHelperTests
{
    [Test]
    public void IsValidEnumValue_WithInValidValueType_ReturnsFalse()
    {
        // Arrange
        var value = OrganisationStatus.Active;
        // Act
        var result = EnumValidationHelper.IsValidEnumValue(value, typeof(Domain.Entities.Organisation));
        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void IsValidEnumValue_WithValidEnumValueType_ReturnsTrue()
    {
        // Arrange
        var value = OrganisationStatus.Active;
        // Act
        var result = EnumValidationHelper.IsValidEnumValue(value, typeof(OrganisationStatus));
        // Assert
        Assert.IsTrue(result);
    }

    [TestCase("1")]
    [TestCase("Active")]
    public void IsValidEnumValue_WithValidValue_ReturnsTrue(string value)
    {
        // Arrange
        // Act
        var result = EnumValidationHelper.IsValidEnumValue<OrganisationStatus>(value);
        // Assert
        Assert.IsTrue(result);
    }

    [TestCase("10")]
    [TestCase("InvalidStatus")]
    public void IsValidEnumValue_WithInvalidValue_ReturnsFalse(string value)
    {
        // Arrange
        // Act
        var result = EnumValidationHelper.IsValidEnumValue<OrganisationStatus>(value);
        // Assert
        Assert.IsFalse(result);
    }
}
