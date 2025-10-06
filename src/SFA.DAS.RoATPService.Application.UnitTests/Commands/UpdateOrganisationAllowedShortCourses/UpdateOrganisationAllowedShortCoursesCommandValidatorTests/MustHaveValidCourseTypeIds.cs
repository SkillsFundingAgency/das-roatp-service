using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.UpdateOrganisationAllowedShortCourses.UpdateOrganisationAllowedShortCoursesCommandValidatorTests;

public class MustHaveValidCourseTypeIds
{
    [Test, RecursiveMoqAutoData]
    public async Task CourseTypeIds_IsEmpty_FailsValidation(
        UpdateOrganisationAllowedShortCoursesCommandValidator sut,
        int ukprn,
        string userId)
    {
        UpdateOrganisationAllowedShortCoursesCommand command = new(ukprn, [], userId);

        var result = await sut.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.CourseTypeIds).WithErrorMessage(UpdateOrganisationAllowedShortCoursesCommandValidator.CourseTypeIdsIsRequiredMessage);
    }

    [Test, RecursiveMoqAutoData]
    public async Task CourseTypeIds_ContainsInvalidId_FailsValidation(
        [Frozen] Mock<ICourseTypesRepository> courseTypesRepositoryMock,
        UpdateOrganisationAllowedShortCoursesCommandValidator sut,
        int ukprn,
        string userId)
    {
        UpdateOrganisationAllowedShortCoursesCommand command = new(ukprn, [999], userId);
        courseTypesRepositoryMock.Setup(r => r.GetAllCourseTypes(default)).ReturnsAsync([]);

        var result = await sut.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.CourseTypeIds).WithErrorMessage(UpdateOrganisationAllowedShortCoursesCommandValidator.InvalidCourseTypeIdMessage);
    }

    [Test, RecursiveMoqAutoData]
    public async Task CourseTypeIds_ContainsValidId_PassesValidation(
        [Frozen] Mock<ICourseTypesRepository> courseTypesRepositoryMock,
        UpdateOrganisationAllowedShortCoursesCommandValidator sut,
        int ukprn,
        string userId)
    {
        UpdateOrganisationAllowedShortCoursesCommand command = new(ukprn, [1], userId);
        courseTypesRepositoryMock.Setup(r => r.GetAllCourseTypes(default)).ReturnsAsync([new CourseType { Id = 1, LearningType = LearningType.ShortCourse }]);

        var result = await sut.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.CourseTypeIds);
    }
}
