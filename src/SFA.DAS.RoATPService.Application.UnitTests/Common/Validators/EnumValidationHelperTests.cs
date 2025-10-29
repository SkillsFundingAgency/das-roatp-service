using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Common.Validators;

namespace SFA.DAS.RoATPService.Application.UnitTests.Common.Validators;
public class EnumValidationHelperTests
{
    [Test]
    public void IsValidEnumValue_WithInValidValueType_ReturnsFalse()
    {
        // Arrange
        var value = Domain.Entities.OrganisationStatus.Active;
        // Act
        var result = EnumValidationHelper.IsValidEnumValue(value, typeof(Domain.Entities.Organisation));
        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void IsValidEnumValue_WithValidEnumValueType_ReturnsTrue()
    {
        // Arrange
        var value = Domain.Entities.OrganisationStatus.Active;
        // Act
        var result = EnumValidationHelper.IsValidEnumValue(value, typeof(Domain.Entities.OrganisationStatus));
        // Assert
        Assert.IsTrue(result);
    }

    [TestCase("1")]
    [TestCase("Active")]
    public void IsValidEnumValue_WithValidValue_ReturnsTrue(string value)
    {
        // Arrange
        // Act
        var result = EnumValidationHelper.IsValidEnumValue<Domain.Entities.OrganisationStatus>(value);
        // Assert
        Assert.IsTrue(result);
    }

    [TestCase("10")]
    [TestCase("InvalidStatus")]
    public void IsValidEnumValue_WithInvalidValue_ReturnsFalse(string value)
    {
        // Arrange
        // Act
        var result = EnumValidationHelper.IsValidEnumValue<Domain.Entities.OrganisationStatus>(value);
        // Assert
        Assert.IsFalse(result);
    }
}
