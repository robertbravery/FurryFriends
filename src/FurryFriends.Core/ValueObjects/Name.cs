using FluentValidation;
using FluentValidation.Results;

namespace FurryFriends.Core.ValueObjects;

public class Name : ValueObject
{
  public string FirstName { get; }
  public string LastName { get; }
  public string FullName { get; }

  private Name(string firstName, string lastName, string fullName)
  {
    FirstName = firstName;
    LastName = lastName;
    FullName = fullName;
  }

  public static Result<Name> Create(string firstName, string lastName, string fullName, IValidator<Name> validator)
  {
    var name = new Name(firstName, lastName, fullName);

    var validationResult = validator.Validate(name);
    return validationResult.IsValid ? Result.Success(name) : Result.Error(validationResult.ToString());
  }

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return FirstName;
    yield return LastName;
  }
}
