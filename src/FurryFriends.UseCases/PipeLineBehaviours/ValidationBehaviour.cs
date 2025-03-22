using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace FurryFriends.UseCases.PipeLineBehaviours;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IResult
{
  private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    if (!_validators.Any())
    {
      return await next();
    }

    var context = new ValidationContext<TRequest>(request);
    var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
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

    return await next();
  }
}
