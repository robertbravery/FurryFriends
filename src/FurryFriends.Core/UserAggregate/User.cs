using FurryFriends.Core.GuardClauses;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.Core.Entities;

public class User : IAggregateRoot
{
  public Guid Id { get; private set; } = default!;
  public string Name { get; private set; } = default!;
  public string Email { get; private set; } = default!;
  public PhoneNumber PhoneNumber { get; private set; } = default!;
  public Address Address { get; private set; } = default!;

  public User()
  {

  }
  public User(string name, string email, PhoneNumber phoneNumber, Address address)
  {
    // Guard clauses to ensure valid input
    Guard.Against.NullOrWhiteSpace(name, nameof(name));
    Guard.Against.NullOrWhiteSpace(email, nameof(email));
    Guard.Against.InvalidEmail(email, nameof(email));
    Guard.Against.Null(address, nameof(address));

    Id = Guid.NewGuid();
    Name = name;
    Email = email;
    PhoneNumber = phoneNumber;
    Address = address;
  }

  public void UpdateDetails(string name, string email, PhoneNumber phoneNumber, Address address)
  {
    // Guard clauses to ensure valid input
    Guard.Against.NullOrEmpty(name, nameof(name));
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

  public void UpdateUsername(string newUserName)
  {
    Guard.Against.NullOrEmpty(newUserName, nameof(newUserName));
    Guard.Against.StringTooLong(newUserName, 30, nameof(newUserName));
    Guard.Against.StringTooShort(newUserName,5, nameof(newUserName));
    Name = newUserName;
  }
}
