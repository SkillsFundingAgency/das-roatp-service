using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.PatchOrganisation;
using SFA.DAS.RoATPService.Domain.Common;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.PatchOrganisation.PatchOrganisationCommandValidatorTests;

public class PatchOrganisationCommandValidatorPatchDocTests
{
    [Test]
    public async Task Validate_PatchDoc_ShouldNotBeEmpty()
    {
        //Arrange
        var command = new PatchOrganisationCommand(PatchOrganisationCommandValidatorTestHelper.ValidUkprn, PatchOrganisationCommandValidatorTestHelper.ValidUserId, new());
        var sut = PatchOrganisationCommandValidatorTestHelper.GetValidator();
        //Act
        var result = await sut.TestValidateAsync(command);
        //Assert
        result.ShouldHaveValidationErrorFor(c => c.PatchDoc.Operations).WithErrorMessage(PatchOrganisationCommandValidator.PatchDocCannotBeEmptyErrorMessage);
    }

    [TestCase(PatchOrganisationCommandValidatorHelper.ProviderTypePath, true)]
    [TestCase(PatchOrganisationCommandValidatorHelper.OrganisationTypeIdPath, true)]
    [TestCase(PatchOrganisationCommandValidatorHelper.StatusPath, true)]
    [TestCase(PatchOrganisationCommandValidatorHelper.RemovedReasonIdPath, true)]
    [TestCase("/ukprn", false)]
    public async Task Validate_PatchDoc_ShouldHaveExpectedPathsOnly(string path, bool isValid)
    {
        //Arrange
        JsonPatchDocument<PatchOrganisationModel> patchDoc = new();
        patchDoc.Operations.Add(new Operation<PatchOrganisationModel>
        {
            op = OperationType.Replace.ToString(),
            path = path,
            value = 1
        });
        var command = new PatchOrganisationCommand(PatchOrganisationCommandValidatorTestHelper.ValidUkprn, PatchOrganisationCommandValidatorTestHelper.ValidUserId, patchDoc);
        var sut = new PatchOrganisationCommandValidator(PatchOrganisationCommandValidatorTestHelper.GetRemovedReasonsRepositoryMock().Object, PatchOrganisationCommandValidatorTestHelper.GetOganisationTypesRepositoryMock().Object, PatchOrganisationCommandValidatorTestHelper.GetOrganisationRepositoryMock(OrganisationStatus.Removed).Object);
        //Act
        var result = await sut.TestValidateAsync(command);
        //Assert
        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(c => c.PatchDoc.Operations);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.PatchDoc.Operations).WithErrorMessage(PatchOrganisationCommandValidator.PatchDocUnexpectedPathErrorMessage);
        }
    }

    [TestCase(OperationType.Replace, true)]
    [TestCase(OperationType.Remove, false)]
    [TestCase(OperationType.Add, false)]
    public async Task Validate_PatchDoc_ShouldHaveReplaceOperationOnly(OperationType operationType, bool isValid)
    {
        //Arrange
        JsonPatchDocument<PatchOrganisationModel> patchDoc = new();
        patchDoc.Operations.Add(new Operation<PatchOrganisationModel>
        {
            op = operationType.ToString(),
            path = PatchOrganisationCommandValidatorHelper.StatusPath,
            value = "Active"
        });
        var command = new PatchOrganisationCommand(PatchOrganisationCommandValidatorTestHelper.ValidUkprn, PatchOrganisationCommandValidatorTestHelper.ValidUserId, patchDoc);
        var sut = PatchOrganisationCommandValidatorTestHelper.GetValidator();
        //Act
        var result = await sut.TestValidateAsync(command);
        //Assert
        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(c => c.PatchDoc.Operations);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.PatchDoc.Operations).WithErrorMessage(PatchOrganisationCommandValidator.PatchDocOnlyReplaceOperationExpectedErrorMessage);
        }
    }

    [Test]
    public async Task Validate_PatchDoc_ShouldNotHaveDuplicatePaths()
    {
        //Arrange
        JsonPatchDocument<PatchOrganisationModel> patchDoc = new();
        patchDoc.Replace(c => c.Status, OrganisationStatus.Active);
        patchDoc.Replace(c => c.Status, OrganisationStatus.Removed);

        var command = new PatchOrganisationCommand(PatchOrganisationCommandValidatorTestHelper.ValidUkprn, PatchOrganisationCommandValidatorTestHelper.ValidUserId, patchDoc);
        var sut = PatchOrganisationCommandValidatorTestHelper.GetValidator();
        //Act
        var result = await sut.TestValidateAsync(command);
        //Assert
        result.ShouldHaveValidationErrorFor(c => c.PatchDoc.Operations).WithErrorMessage(PatchOrganisationCommandValidator.PatchDocDuplicatePathNotAllowedErrorMessage);
    }

    [TestCase("Removed", true)]
    [TestCase("Active", true)]
    [TestCase("ActiveNoStarts", true)]
    [TestCase("OnBoarding", true)]
    [TestCase("0", true)]
    [TestCase("1", true)]
    [TestCase("2", true)]
    [TestCase("3", true)]
    [TestCase("4", false)]
    [TestCase("InvalidStatus", false)]
    public async Task Validate_PatchDoc_StatusShouldBeValid(string statusValue, bool isValid)
    {
        //Arrange
        JsonPatchDocument<PatchOrganisationModel> patchDoc = new();
        patchDoc.Operations.Add(new Operation<PatchOrganisationModel>
        {
            op = OperationType.Replace.ToString(),
            path = PatchOrganisationCommandValidatorHelper.StatusPath,
            value = statusValue
        });

        if (statusValue == "0" || statusValue == "Removed") patchDoc.Replace(c => c.RemovedReasonId, 1);

        var command = new PatchOrganisationCommand(PatchOrganisationCommandValidatorTestHelper.ValidUkprn, PatchOrganisationCommandValidatorTestHelper.ValidUserId, patchDoc);
        var sut = new PatchOrganisationCommandValidator(PatchOrganisationCommandValidatorTestHelper.GetRemovedReasonsRepositoryMock().Object, PatchOrganisationCommandValidatorTestHelper.GetOganisationTypesRepositoryMock().Object, PatchOrganisationCommandValidatorTestHelper.GetOrganisationRepositoryMock(OrganisationStatus.Removed).Object);
        //Act
        var result = await sut.TestValidateAsync(command);
        //Assert
        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(c => c.PatchDoc.Operations);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.PatchDoc.Operations).WithErrorMessage(PatchOrganisationCommandValidator.PatchDocStatusShouldBeValidErrorMessage);
        }
    }

    [TestCase("Main", true)]
    [TestCase("Employer", true)]
    [TestCase("Supporting", true)]
    [TestCase("1", true)]
    [TestCase("2", true)]
    [TestCase("3", true)]
    [TestCase("4", false)]
    [TestCase("InvalidType", false)]
    public async Task Validate_PatchDoc_ProviderTypeShouldBeValid(string providerType, bool isValid)
    {
        //Arrange
        JsonPatchDocument<PatchOrganisationModel> patchDoc = new();
        patchDoc.Operations.Add(new Operation<PatchOrganisationModel>
        {
            op = OperationType.Replace.ToString(),
            path = PatchOrganisationCommandValidatorHelper.ProviderTypePath,
            value = providerType
        });
        var command = new PatchOrganisationCommand(PatchOrganisationCommandValidatorTestHelper.ValidUkprn, PatchOrganisationCommandValidatorTestHelper.ValidUserId, patchDoc);
        var sut = PatchOrganisationCommandValidatorTestHelper.GetValidator();
        //Act
        var result = await sut.TestValidateAsync(command);
        //Assert
        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(c => c.PatchDoc.Operations);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.PatchDoc.Operations).WithErrorMessage(PatchOrganisationCommandValidator.PatchDocProviderTypeShouldBeValidErrorMessage);
        }
    }

    [Test]
    public async Task Validate_PatchDoc_RemovedStatusShouldHaveRemovedReason()
    {
        //Arrange
        JsonPatchDocument<PatchOrganisationModel> patchDoc = new();
        patchDoc.Replace(c => c.Status, OrganisationStatus.Removed);

        var command = new PatchOrganisationCommand(PatchOrganisationCommandValidatorTestHelper.ValidUkprn, PatchOrganisationCommandValidatorTestHelper.ValidUserId, patchDoc);

        var sut = PatchOrganisationCommandValidatorTestHelper.GetValidator();
        //Act
        var result = await sut.TestValidateAsync(command);
        //Assert
        result.ShouldHaveValidationErrorFor(c => c.PatchDoc.Operations).WithErrorMessage(PatchOrganisationCommandValidator.PatchDocRemovedStatusShouldAccompanyRemovedReasonIdErrorMessage);
    }

    [TestCase(OrganisationStatus.OnBoarding)]
    [TestCase(OrganisationStatus.Active)]
    [TestCase(OrganisationStatus.ActiveNoStarts)]
    public async Task Validate_PatchDoc_OtherThanRemovedStatusShouldNotHaveRemovedReason(OrganisationStatus status)
    {
        //Arrange
        JsonPatchDocument<PatchOrganisationModel> patchDoc = new();
        patchDoc.Replace(c => c.Status, status);
        patchDoc.Replace(c => c.RemovedReasonId, 1);

        var command = new PatchOrganisationCommand(PatchOrganisationCommandValidatorTestHelper.ValidUkprn, PatchOrganisationCommandValidatorTestHelper.ValidUserId, patchDoc);

        var sut = PatchOrganisationCommandValidatorTestHelper.GetValidator();
        //Act
        var result = await sut.TestValidateAsync(command);
        //Assert
        result.ShouldHaveValidationErrorFor(c => c.PatchDoc.Operations).WithErrorMessage(PatchOrganisationCommandValidator.PatchDocOtherThanRemovedStatusShouldNotHaveRemovedReasonOperationErrorMessage);
    }

    [TestCase(2, true)]
    [TestCase(3, false)]
    public async Task Validate_PatchDoc_RemovedReasonIdShouldBeValid(int removedReasonId, bool isValid)
    {
        //Arrange
        JsonPatchDocument<PatchOrganisationModel> patchDoc = new();
        patchDoc.Replace(c => c.Status, OrganisationStatus.Removed);
        patchDoc.Replace(c => c.RemovedReasonId, removedReasonId);

        var command = new PatchOrganisationCommand(PatchOrganisationCommandValidatorTestHelper.ValidUkprn, PatchOrganisationCommandValidatorTestHelper.ValidUserId, patchDoc);

        var sut = PatchOrganisationCommandValidatorTestHelper.GetValidator();
        //Act
        var result = await sut.TestValidateAsync(command);
        //Assert
        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(c => c.PatchDoc.Operations);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.PatchDoc.Operations).WithErrorMessage(PatchOrganisationCommandValidator.PatchDocRemovedReasonIdShouldBeValidErrorMessage);
        }
    }

    [Test]
    public async Task Validate_PatchDoc_RemovedReasonIdIsNotValidIfOrganisationStatusIsNotRemoved()
    {
        //Arrange
        JsonPatchDocument<PatchOrganisationModel> patchDoc = new();
        patchDoc.Replace(c => c.RemovedReasonId, 1);

        var command = new PatchOrganisationCommand(PatchOrganisationCommandValidatorTestHelper.ValidUkprn, PatchOrganisationCommandValidatorTestHelper.ValidUserId, patchDoc);

        var sut = PatchOrganisationCommandValidatorTestHelper.GetValidator();
        //Act
        var result = await sut.TestValidateAsync(command);
        //Assert
        result.ShouldHaveValidationErrorFor(c => c.PatchDoc.Operations).WithErrorMessage(PatchOrganisationCommandValidator.PatchDocRemovedReasonIdIsNotValidIfOrganisationStatusIsNotRemovedErrorMessage);
    }

    [TestCase(2, true)]
    [TestCase(3, false)]
    public async Task Validate_PatchDoc_OrganisationTypeIdShouldBeValid(int organisationTypeId, bool isValid)
    {
        //Arrange
        JsonPatchDocument<PatchOrganisationModel> patchDoc = new();
        patchDoc.Replace(c => c.OrganisationTypeId, organisationTypeId);

        var command = new PatchOrganisationCommand(PatchOrganisationCommandValidatorTestHelper.ValidUkprn, PatchOrganisationCommandValidatorTestHelper.ValidUserId, patchDoc);

        var sut = PatchOrganisationCommandValidatorTestHelper.GetValidator();
        //Act
        var result = await sut.TestValidateAsync(command);
        //Assert
        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(c => c.PatchDoc.Operations);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.PatchDoc.Operations).WithErrorMessage(PatchOrganisationCommandValidator.PatchDocOrganisationTypeIdShouldBeValidErrorMessage);
        }
    }
}
