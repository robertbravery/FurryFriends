
using FurryFriends.Core.ValueObjects.Validators;

namespace FurryFriends.Core.ValueObjects;

public class DateOfBirth: ValueObject
{
  public DateTime Date { get; private set; }

  private DateOfBirth(DateTime date)
  {
    Date = date;
  }

  public static Result<DateOfBirth> Create(DateTime dateOfBirth)
  {
    var dateOfBirthValidator = new DateOfBirthValidator();
    var dateOfBirthObject = new DateOfBirth(dateOfBirth);

    var validationResults = dateOfBirthValidator.Validate(dateOfBirthObject);
    return validationResults.IsValid
      ? Result.Success(dateOfBirthObject)
      : Result.Error(validationResults.ToString());
  }

  // Override equality operators
  public override bool Equals(object? obj)
  {
    if (obj is DateOfBirth other)
    {
      return Date == other.Date;
    }
    return false;
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(Date);
  }

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return Date;
  }
}
