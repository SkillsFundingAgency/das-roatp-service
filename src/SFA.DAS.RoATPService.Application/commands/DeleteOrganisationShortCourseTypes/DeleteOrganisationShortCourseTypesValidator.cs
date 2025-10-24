using FluentValidation;
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
    }
}