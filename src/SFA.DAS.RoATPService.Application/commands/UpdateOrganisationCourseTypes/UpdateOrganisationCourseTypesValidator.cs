using System.Linq;
using FluentValidation;
using SFA.DAS.RoATPService.Application.Common.Validators;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;

public class UpdateOrganisationCourseTypesValidator : AbstractValidator<UpdateOrganisationCourseTypesCommand>
{
    public const string CourseTypeIdsIsRequiredMessage = "At least one CourseTypeId is required";
    public const string InvalidCourseTypeIdMessage = "Request contains invalid course types";
    public const string RequestingUserIdIsRequiredMessage = "RequestingUserId must not be empty";

    public UpdateOrganisationCourseTypesValidator(ICourseTypesRepository courseTypesRepository)
    {
        RuleFor(c => c.Ukprn)
            .Cascade(CascadeMode.Stop)
            .UkprnNotEmpty()
            .MustBeValidUkprnFormat();

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
