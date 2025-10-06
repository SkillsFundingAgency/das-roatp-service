using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.RoATPService.Application.Mediatr.Behaviors;

[ExcludeFromCodeCoverage]
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TResponse : ValidatedResponse
    where TRequest : IRequest<TResponse>
{
    private readonly IValidator<TRequest> _compositeValidator;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(IValidator<TRequest> compositeValidator, ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _compositeValidator = compositeValidator;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Validating Roatp API request");

        var result = await _compositeValidator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            var responseType = typeof(TResponse);

            if (responseType.IsGenericType)
            {
                var resultType = responseType.GetGenericArguments()[0];
                responseType = typeof(ValidatedResponse<>).MakeGenericType(resultType);
            }

            var errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));

            if (Activator.CreateInstance(responseType, errors) as TResponse is { } invalidResponse)
            {
                return invalidResponse;
            }
        }

        _logger.LogTrace("Validation passed");

        var response = await next(cancellationToken);

        return response;
    }
}
