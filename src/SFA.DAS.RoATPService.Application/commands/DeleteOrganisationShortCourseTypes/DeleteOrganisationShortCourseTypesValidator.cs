using FluentValidation;
using SFA.DAS.RoATPService.Application.Common.Validators;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.commands.DeleteOrganisationShortCourseTypes;
public class DeleteOrganisationShortCourseTypesValidator : AbstractValidator<DeleteOrganisationShortCourseTypesCommand>
{
    public const string UkprnIsRequiredMessage = "Ukprn must not be empty";

    public DeleteOrganisationShortCourseTypesValidator(IOrganisationsRepository organisationRepository)
    {
        RuleFor(c => c.Ukprn)
            .GreaterThan(0)
            .WithMessage(UkprnIsRequiredMessage);
        RuleFor(c => c.Ukprn)
            .IsValidUkprnFormat()
            .When(c => c.Ukprn > 0);
    }
}