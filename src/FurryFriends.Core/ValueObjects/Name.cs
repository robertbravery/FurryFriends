using FluentValidation;

namespace FurryFriends.Core.ValueObjects;

public class Name : ValueObject
{
  public string FirstName { get; }
  public string LastName { get; }
  public string FullName => $"{FirstName} {LastName}";

  private Name(string firstName, string lastName)
  {
    FirstName = firstName;
    LastName = lastName;
  }

  public static Result<Name> Create(string firstName, string lastName, IValidator<Name> validator)
  {
    var name = new Name(firstName, lastName);

    var validationResult = validator.Validate(name);
    return validationResult.IsValid ? Result.Success(name) : Result.Error(validationResult.ToString());
  }

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return FirstName;
    yield return LastName;
  }
  public override string ToString() { return FullName; }

  public override bool Equals(object? obj)
  {
    if (obj is Name other)
    {
      return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }
    return false;
  }

  public override int GetHashCode()
  {
    unchecked
    {
      int hash = 17;
      hash = hash * 23 + FirstName.GetHashCode();
      hash = hash * 23 + LastName.GetHashCode();
      return hash;
    }
  }
}
