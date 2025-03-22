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

  public async Task<Result> SetPhoneNumber(string phoneNumber, IValidator<PhoneNumber> validator)
  {
    var result = await PhoneNumber.Create(string.Empty, phoneNumber);
    if (result.IsSuccess)
    {
      PhoneNumber = result.Value;
      return Result.Success();
    }
    return Result.Error(new ErrorList(result.Errors.ToArray()));
  }

  public void UpdateName(Name name)
  {
    Name = name;
  }
}
