using FluentValidation.Results;

namespace FurryFriends.Core.Extensions;
public static class ValidationFailureExtensions
{
  public static Result ToInvalidValidationErrorResult(this List<ValidationFailure> failures)
  {
    return Result.Invalid(failures.Select(e => new ValidationError
    { ErrorMessage = e.ErrorMessage }).ToArray());

  }
}
