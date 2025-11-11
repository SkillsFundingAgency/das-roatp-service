using FluentValidation;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisationTypes;

public class GetOrganisationTypesQueryValidator : AbstractValidator<GetOrganisationTypesQuery>
{
    public const string InvalidProviderTypeIdErrorMessage = "ProviderTypeId should be one of 1,2,3";

    public GetOrganisationTypesQueryValidator()
    {
        RuleFor(q => q.ProviderTypeId)
            .InclusiveBetween(1, 3)
            .WithMessage(InvalidProviderTypeIdErrorMessage)
            .When(q => q.ProviderTypeId.HasValue);
    }
}
