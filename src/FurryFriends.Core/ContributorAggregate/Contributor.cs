using Ardalis.Result;
using FluentValidation;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.Core.ContributorAggregate;

public class Contributor : EntityBase, IAggregateRoot
{
  public Contributor()
  {

  }
  public Contributor(Name name)
  {
    Name = name;
  }
  // Example of validating primary constructor inputs.
  // See: https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/primary-constructors#initialize-base-class
  public Name Name { get; private set; } = default!;
  public ContributorStatus Status { get; private set; } = ContributorStatus.NotSet;
  public PhoneNumber? PhoneNumber { get; private set; }

  public void SetPhoneNumber(string phoneNumber, IValidator<PhoneNumber> validator) => PhoneNumber = PhoneNumber.Create(string.Empty, phoneNumber, string.Empty,  validator);

  public void UpdateName(Name name)
  {
    Name = name;
  }
}
