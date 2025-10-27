using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.RoATPService.Application.Mediatr.Behaviors;

/// <summary>
/// Use this class to return validation results only, 
/// where there is no other result object is being returned
/// for example a command does not need to return any results when successful.
/// </summary>
public class ValidatedResponse
{
    public IEnumerable<ValidationError> Errors { init; get; } = [];

    public bool IsValidResponse => !Errors.Any();

    public ValidatedResponse(IEnumerable<ValidationError> validationErrors) => Errors = validationErrors;

    public ValidatedResponse() { }
}

/// <summary>
/// Use this class to return validation results along with a result object
/// </summary>
/// <typeparam name="T"></typeparam>
public class ValidatedResponse<T> : ValidatedResponse
{
    public T Result { get; }

    public ValidatedResponse(T model) => Result = model;

    public ValidatedResponse(IEnumerable<ValidationError> validationErrors) : base(validationErrors) { Result = default; }
}

public record ValidationError(string PropertyName, string ErrorMessage)
{
    public static implicit operator ValidationError(ValidationFailure error) => new(error.PropertyName, error.ErrorMessage);
}
