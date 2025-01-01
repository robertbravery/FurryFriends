using FurryFriends.Core.GuardClauses;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.Core.Entities;

public class User : IAggregateRoot
{
  public Guid Id { get; private set; } = default!;
  public Name Name { get; private set; } = default!;
  public string Email { get; private set; } = default!;
  public PhoneNumber PhoneNumber { get; private set; } = default!;
  public Address Address { get; private set; } = default!;

  public User()
  {

  }
  public User(Name name, string email, PhoneNumber phoneNumber, Address address)
  {
    // Guard clauses to ensure valid input
    Guard.Against.NullOrWhiteSpace(name.FirstName, nameof(name.FirstName));
    Guard.Against.NullOrWhiteSpace(email, nameof(email));
    Guard.Against.InvalidEmail(email, nameof(email));
    Guard.Against.Null(address, nameof(address));

    Id = Guid.NewGuid();
    Name = name;
    Email = email;
    PhoneNumber = phoneNumber;
    Address = address;
  }

  public void UpdateDetails(Name name, string email, PhoneNumber phoneNumber, Address address)
  {
    // Guard clauses to ensure valid input
    Guard.Against.NullOrEmpty(name.FirstName, nameof(name.FullName));
    Guard.Against.NullOrEmpty(email, nameof(email));
    Guard.Against.InvalidEmail(email, nameof(email));
    Guard.Against.Null(address, nameof(address));

    Name = name;
    Email = email;
    PhoneNumber = phoneNumber;
    Address = address;
  }

  /// <summary>
  /// Updates the email address of the user.
  /// </summary>
  /// <param name="newEmail">The new email address.</param>
  public void UpdateEmail(string newEmail)
  {
    Guard.Against.InvalidEmail(newEmail, nameof(newEmail));
    Email = newEmail;
  }

  public void Deactivate()
  {
    // Deactivation logic can be added here
  }

  public void UpdateUsername(Name newUserName)
  {
    Guard.Against.NullOrEmpty(newUserName.FirstName, nameof(newUserName.FirstName));
    Guard.Against.StringTooLong(newUserName.FirstName, 30, nameof(newUserName.FirstName));
    Guard.Against.StringTooShort(newUserName.FirstName,5, nameof(newUserName.FirstName));
    Name = newUserName;
  }
}
