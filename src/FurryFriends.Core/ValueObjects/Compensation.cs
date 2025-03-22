using FurryFriends.Core.Extensions;
using FurryFriends.Core.ValueObjects.Validators;

namespace FurryFriends.Core.ValueObjects;

public class Compensation : ValueObject
{
  public decimal HourlyRate { get; }
  public string Currency { get; } = default!;

  private Compensation()
  {

  }

  private Compensation(decimal amount, string currency)
  {
    HourlyRate = amount;
    Currency = currency;
  }

  public static Result<Compensation> Create(decimal amount, string currency)
  {
    var compensation = new Compensation(amount, currency);
    var validator = new CompensationValidator();
    var validationResult = validator.Validate(compensation);

    return validationResult.IsValid
      ? Result.Success(compensation)
      : validationResult.Errors.ToInvalidValidationErrorResult();

  }

  // Equality members
  protected bool Equals(Compensation other)
  {
    return HourlyRate == other.HourlyRate && string.Equals(Currency, other.Currency, StringComparison.OrdinalIgnoreCase);
  }

  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(null, obj)) return false;
    if (ReferenceEquals(this, obj)) return true;
    if (obj.GetType() != GetType()) return false;
    return Equals((Compensation)obj);
  }

  public override int GetHashCode()
  {
    unchecked
    {
      return (HourlyRate.GetHashCode() * 397) ^ (Currency != null
        ? StringComparer.OrdinalIgnoreCase.GetHashCode(Currency)
        : 0);
    }
  }

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return HourlyRate;
    yield return Currency;
  }
}

