using FluentValidation;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.commands.DeleteOrganisationShortCourseTypes;
public class DeleteOrganisationShortCourseTypesValidator : AbstractValidator<DeleteOrganisationShortCourseTypesCommand>
{
    public const string InvalidUkprnMessage = "Ukprn does not exist";
    public const string UkprnIsRequiredMessage = "Ukprn must not be empty";
    public const string RequestingUserIdIsRequiredMessage = "RequestingUserId must not be empty";

    public DeleteOrganisationShortCourseTypesValidator(IOrganisationsRepository organisationRepository)
    {
        RuleFor(c => c.Ukprn)
            .GreaterThan(0)
            .WithMessage(UkprnIsRequiredMessage)
            .MustAsync(async (id, token) =>
            {
                var org = await organisationRepository.GetOrganisationByUkprn(id, token);
                return org != null;
            })
            .WithMessage(InvalidUkprnMessage);
        RuleFor(c => c.RequestingUserId)
            .NotEmpty()
            .WithMessage(RequestingUserIdIsRequiredMessage);
    }
}
