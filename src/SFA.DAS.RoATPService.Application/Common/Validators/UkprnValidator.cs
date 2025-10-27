using FluentValidation;

namespace SFA.DAS.RoATPService.Application.Common.Validators;
public static class UkprnValidator
{
    public const string UkprnFormatValidationMessage = "Currently a Ukprn must start with the value 1 and should be 8 digits long.";

    public static IRuleBuilderOptions<T, int> IsValidUkprnFormat<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .Must(ukprn => ukprn.ToString()!.StartsWith('1') && ukprn.ToString()!.Length == 8)
            .WithMessage(UkprnFormatValidationMessage);
    }
}