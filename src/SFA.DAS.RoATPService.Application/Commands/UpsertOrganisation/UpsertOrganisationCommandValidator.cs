using System;
using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using SFA.DAS.RoATPService.Application.Common.Validators;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.Commands.UpsertOrganisation;

public class UpsertOrganisationCommandValidator : AbstractValidator<UpsertOrganisationCommand>
{
    private const string CompaniesHouseNumberRegex = "[A-Za-z0-9]{2}[0-9]{4}[A-Za-z0-9]{2}";
    private const string CharityNumberInvalidCharactersRegex = "[^a-zA-Z0-9\\-]";

    public const string UkprnAlreadyOnRegistertErrorMessage = "Provider with Ukprn is already on the register";
    public const string UkprnNotOnRegisterErrorMessage = "Provider with Ukprn not found on the register";
    public const string LegalNameIsRequiredErrorMessage = "Legal name is required";
    public const string LegalNameTooLongErrorMessage = "Legal name should be 200 characters or less";
    public const string TradingNameTooLongErrorMessage = "Trading name should be 200 characters or less";
    public const string OrganisationTypeIdIsRequiredErrorMessage = "OrganisationTypeId is required";
    public const string OrganisationTypeIdShouldBeValidErrorMessage = "Invalid OrganisationTypeId";
    public const string RequestingUserIdRequiredErrorMessage = "Requesting user id is required";

    public const string ProviderTypeIsRequiredErrorMessage = "ProviderType is required";
    public const string CompanyNumberIsInvalidErrorMessage = "Company number is an invalid format";
    public const string CompanyNumberIsUsedErrorMessage = "Company number is already used for an existing organisation";

    public const string CharityNumberIsInvalidErrorMessage = "Charity number is an invalid format";
    public const string CharityNumberIsUsedErrorMessage = "Charity number is already used for an existing organisation";

    public UpsertOrganisationCommandValidator(IOrganisationsRepository organisationsRepository, IOrganisationTypesRepository organisationTypesRepository)
    {
        RuleFor(x => x.Ukprn)
            .Cascade(CascadeMode.Stop)
            .MustBeValidUkprnFormat()
            .MustAsync(async (command, ukprn, cancellationToken) =>
            {
                var org = await organisationsRepository.GetOrganisationByUkprn(ukprn, cancellationToken);
                return command.IsNewOrganisation ? org == null : org != null;
            })
            .WithMessage((command, ukprn) => command.IsNewOrganisation ? UkprnAlreadyOnRegistertErrorMessage : UkprnNotOnRegisterErrorMessage);

        RuleFor(c => c.LegalName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(LegalNameIsRequiredErrorMessage)
            .Length(1, 200)
            .WithMessage(LegalNameTooLongErrorMessage);

        RuleFor(c => c.TradingName)
            .Length(0, 200)
            .WithMessage(TradingNameTooLongErrorMessage);

        RuleFor(x => x.ProviderType)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ProviderTypeIsRequiredErrorMessage);

        RuleFor(x => x.OrganisationTypeId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(OrganisationTypeIdIsRequiredErrorMessage)
            .MustAsync(async (organisationTypeId, cancellationToken) =>
            {
                var organisationTypes = await organisationTypesRepository.GetOrganisationTypes(cancellationToken);
                return organisationTypes.Select(r => r.Id.ToString()).Contains(organisationTypeId.ToString());
            })
            .WithMessage(OrganisationTypeIdShouldBeValidErrorMessage);

        RuleFor(c => c.RequestingUserId)
            .NotEmpty()
            .WithMessage(RequestingUserIdRequiredErrorMessage);

        RuleFor(c => c.CompanyNumber)
            .Cascade(CascadeMode.Stop)
            .Must(IsValidCompanyNumber)
            .WithMessage(CompanyNumberIsInvalidErrorMessage)
            .MustAsync(async (command, companyNumber, cancellationToken) =>
            {
                if (!command.IsNewOrganisation) return true;
                if (string.IsNullOrWhiteSpace(companyNumber)) return true;
                var organisations = await organisationsRepository.GetOrganisations(cancellationToken);
                return organisations.All(o => o.CompanyNumber != companyNumber);
            })
            .WithMessage(CompanyNumberIsUsedErrorMessage);

        RuleFor(c => c.CharityNumber)
            .Cascade(CascadeMode.Stop)
            .Must(IsValidCharityNumber)
            .WithMessage(CharityNumberIsInvalidErrorMessage)
            .MustAsync(async (command, charityNumber, cancellationToken) =>
            {
                if (!command.IsNewOrganisation) return true;
                if (string.IsNullOrWhiteSpace(charityNumber)) return true;
                var organisations = await organisationsRepository.GetOrganisations(cancellationToken);
                return organisations.All(o => o.CharityNumber != charityNumber);
            })
            .WithMessage(CharityNumberIsUsedErrorMessage);
    }

    private static bool IsValidCharityNumber(string charityNumber)
    {
        if (string.IsNullOrWhiteSpace(charityNumber))
        {
            return true;
        }

        if (Regex.IsMatch(charityNumber, CharityNumberInvalidCharactersRegex))
        {
            return false;
        }


        if (charityNumber.Length < 6 || charityNumber.Length > 14)
            return false;

        return true;
    }

    private static bool IsValidCompanyNumber(string companyNumber)
    {
        if (string.IsNullOrWhiteSpace(companyNumber))
        {
            return true;
        }

        if (companyNumber.Length != 8 ||
            !Regex.IsMatch(companyNumber, CompaniesHouseNumberRegex))

        {
            return false;
        }

        return true;
    }
}


