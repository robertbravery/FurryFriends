using FurryFriends.Core.ValueObjects.Validators;

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

  public static Result<Name> Create(string firstName, string lastName)
  {
    var name = new Name(firstName, lastName);
    var validator = new NameValidator();
    var validationResult = validator.Validate(name);
    return validationResult.IsValid
      ? Result.Success(name)
      : Result.Error(validationResult.ToString());
  }

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return FirstName;
    yield return LastName;
  }
  public override string ToString() { return FullName; }

  public bool Equals(Name other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;

    return FirstName == other.FirstName && LastName == other.LastName;
  }

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
    return HashCode.Combine(FirstName, LastName);
  }
}

