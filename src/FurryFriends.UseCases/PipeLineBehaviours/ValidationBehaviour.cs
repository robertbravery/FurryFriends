using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.PipeLineBehaviours;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IResult
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        try
        {
            // Use CancellationToken.None to prevent cancellation during validation
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, CancellationToken.None)));
            var failures = validationResults.SelectMany(r => r.Errors).ToList();

            if (failures.Count > 0)
            {
                throw new ValidationException(failures.Select(f => new ValidationFailure
                {
                    PropertyName = f.PropertyName,
                    ErrorMessage = f.ErrorMessage,
                    ErrorCode = f.ErrorCode
                }).ToList());
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Validation was cancelled for request {RequestType}", typeof(TRequest).Name);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during validation for request {RequestType}", typeof(TRequest).Name);
            throw;
        }

        return await next();
    }
}
