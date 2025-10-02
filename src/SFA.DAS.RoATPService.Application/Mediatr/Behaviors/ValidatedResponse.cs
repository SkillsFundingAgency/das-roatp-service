using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace SFA.DAS.RoATPService.Application.Mediatr.Behaviors;

public class ValidatedResponse
{
    public IEnumerable<ValidationError> Errors { init; get; } = [];

    public bool IsValidResponse => !Errors.Any();

    public ValidatedResponse(IEnumerable<ValidationError> validationErrors) => Errors = validationErrors;

    public ValidatedResponse() { }
}

public class ValidatedResponse<T> : ValidatedResponse
{
    public T Result { get; }

    public ValidatedResponse(T model) => Result = model;
}

public record ValidationError(string PropertyName, string ErrorMessage)
{
    public static implicit operator ValidationError(ValidationFailure error) => new(error.PropertyName, error.ErrorMessage);
}
