using FluentValidation;
using SFA.DAS.RoATPService.Domain.Repositories;
using System.Linq;

namespace SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;

public class UpdateOrganisationCourseTypesValidator : AbstractValidator<UpdateOrganisationCourseTypesCommand>
{
    public const string InvalidUkprnMessage = "Ukprn does not exist";
    public const string UkprnIsRequiredMessage = "Ukprn must not be empty";
    public const string CourseTypeIdsIsRequiredMessage = "At least one CourseTypeId is required";
    public const string InvalidCourseTypeIdMessage = "Course type id is not a valid course type";
    public const string RequestingUserIdIsRequiredMessage = "RequestingUserId must not be empty";

    public UpdateOrganisationCourseTypesValidator(IOrganisationsRepository organisationRepository, ICourseTypesRepository courseTypesRepository)
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
        RuleFor(c => c.CourseTypeIds)
            .NotEmpty()
            .WithMessage(CourseTypeIdsIsRequiredMessage)
            .MustAsync(async (courseIds, token) =>
            {
                var courseTypes = await courseTypesRepository.GetAllCourseTypes(token);
                var validCourseIds = courseTypes.Select(c => c.Id);
                return courseIds.All(c => validCourseIds.Contains(c));
            })
            .WithMessage(InvalidCourseTypeIdMessage);
        RuleFor(c => c.RequestingUserId)
            .NotEmpty()
            .WithMessage(RequestingUserIdIsRequiredMessage);
    }
}
