﻿using FurryFriends.Core.GuardClauses;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.Core.UserAggregate;

public class User : EntityBase<Guid>, IAggregateRoot
{

  public Name Name { get; private set; } = default!;
  public string Email { get; private set; } = default!;
  public PhoneNumber PhoneNumber { get; private set; } = default!;
  public Address Address { get; private set; } = default!;
  //public virtual Photo BioPicture { get; private set; } = default!;
  public virtual ICollection<Photo> Photos { get; set; } = new HashSet<Photo>();

  private User()
  {

  }
  private User(Name name, string email, PhoneNumber phoneNumber, Address address)
  {    

    Id = Guid.NewGuid();
    Name = name;
    Email = email;
    PhoneNumber = phoneNumber;
    Address = address;
  }

  public static User Create(Name name, string email, PhoneNumber phoneNumber, Address address)
  {
    // TODO: Use Fluent validation
    Guard.Against.NullOrWhiteSpace(name.FirstName, nameof(name.FirstName));
    Guard.Against.NullOrWhiteSpace(email, nameof(email));
    Guard.Against.InvalidEmail(email, nameof(email));
    Guard.Against.Null(address, nameof(address));
    return new User(name, email, phoneNumber, address);
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
    Guard.Against.StringTooShort(newUserName.FirstName, 5, nameof(newUserName.FirstName));
    Name = newUserName;
  }

  public void AddPhoto(Photo photo)
  {
    Guard.Against.Null(photo, nameof(photo));
    Photos.Add(photo);
  }
}
