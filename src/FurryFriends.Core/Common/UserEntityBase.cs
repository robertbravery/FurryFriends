﻿using FurryFriends.Core.PetWalkerAggregate.Validation;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.Core.Common;

public abstract class UserEntityBase : AuditableEntity<Guid>
{
  public Name Name { get; protected set; } = default!;
  public Email Email { get; protected set; } = default!;
  public PhoneNumber PhoneNumber { get; protected set; } = default!;
  public Address Address { get; protected set; } = default!;


  protected UserEntityBase() { }

  protected UserEntityBase(Name name, Email email, PhoneNumber phoneNumber, Address address)
  {
    Id = Guid.NewGuid();
    Name = name;
    Email = email;
    PhoneNumber = phoneNumber;
    Address = address;
  }

  public void UpdateDetails(Name name, Email email, PhoneNumber phoneNumber, Address address)
  {
    Guard.Against.NullOrEmpty(name.FirstName, nameof(name.FullName));
    Guard.Against.NullOrEmpty(email.EmailAddress, nameof(email));
    Guard.Against.InvalidEmail(email.EmailAddress, nameof(email));
    Guard.Against.Null(address, nameof(address));

    Name = name;
    Email = email;
    PhoneNumber = phoneNumber;
    Address = address;
  }

  public void UpdateEmail(Email newEmail)
  {
    Guard.Against.InvalidEmail(newEmail.EmailAddress, nameof(newEmail));
    Email = newEmail;
  }
  public void UpdateEmail(string newEmail)
  {
    Guard.Against.InvalidEmail(newEmail, nameof(newEmail));
    var email = Email.Create(newEmail);
    Email = email.Value;
  }
}
