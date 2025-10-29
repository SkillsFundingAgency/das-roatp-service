using FluentValidation;

namespace SFA.DAS.RoATPService.Application.Common.Validators;
public static class UkprnValidator
{
    public const string UkprnIsRequiredMessage = "Ukprn is required.";
    public const string UkprnFormatValidationMessage = "Ukprn must start with the value 1 and should be 8 digits long.";

    public static IRuleBuilderOptions<T, int> UkprnNotEmpty<T>(this IRuleBuilder<T, int> ruleBuilder)
        => ruleBuilder.GreaterThan(0).WithMessage(UkprnIsRequiredMessage);

    public static IRuleBuilderOptions<T, int> MustBeValidUkprnFormat<T>(this IRuleBuilder<T, int> ruleBuilder)
        => ruleBuilder
            .Must(ukprn => ukprn.ToString()!.StartsWith('1') && ukprn.ToString()!.Length == 8)
            .WithMessage(UkprnFormatValidationMessage);
}
