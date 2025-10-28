using FluentValidation;
using SFA.DAS.RoATPService.Application.Common.Validators;

namespace SFA.DAS.RoATPService.Application.commands.DeleteOrganisationShortCourseTypes;
public class DeleteOrganisationShortCourseTypesValidator : AbstractValidator<DeleteOrganisationShortCourseTypesCommand>
{
    public DeleteOrganisationShortCourseTypesValidator()
    {
        RuleFor(c => c.Ukprn)
            .Cascade(CascadeMode.Stop)
            .UkprnNotEmpty()
            .MustBeValidUkprnFormat();
    }
}