using FurryFriends.Core.GuardClauses;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.Core.UserAggregate;

public class User : EntityBase<Guid>, IAggregateRoot
{
  public Name Name { get; private set; } = default!;
  public Email Email { get; private set; } = default!;
  public PhoneNumber PhoneNumber { get; private set; } = default!;
  public Address Address { get; private set; } = default!;
  public GenderType Gender { get; private set; } = GenderType.Create(GenderType.GenderCategory.Other).Value;
  public string? Biography { get; private set; }
  public DateTime DateOfBirth { get; private set; }
  public Compensation Compensation { get; private set; } = default!;
  public bool IsActive { get; private set; }
  public bool IsVerified { get; private set; }
  public int YearsOfExperience { get; private set; }
  public bool HasInsurance { get; private set; }
  public bool HasFirstAidCertificatation { get; private set; }
  public int DailyPetWalkLimit { get; private set; }

  public virtual ICollection<Photo> Photos { get; set; } = new HashSet<Photo>();
  public virtual ICollection<ServiceArea> ServiceAreas { get; set; } = new HashSet<ServiceArea>();

  private User() { }

  private User(Name name, Email email, PhoneNumber phoneNumber, Address address)
  {
    Id = Guid.NewGuid();
    Name = name;
    Email = email;
    PhoneNumber = phoneNumber;
    Address = address;
  }

  public static User Create(Name name, Email email, PhoneNumber phoneNumber, Address address)
  {
    Guard.Against.NullOrWhiteSpace(name.FirstName, nameof(name.FirstName));
    Guard.Against.NullOrWhiteSpace(email.EmailAddress, nameof(email));
    Guard.Against.InvalidEmail(email.EmailAddress, nameof(email));
    Guard.Against.Null(address, nameof(address));
    return new User(name, email, phoneNumber, address);
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

  public void Deactivate()
  {
    IsActive = false;
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

  public void UpdateCompensation(Compensation compensation)
  {
    Guard.Against.Null(compensation, nameof(compensation));
    Guard.Against.NegativeOrZero(compensation.HourlyRate, nameof(compensation.HourlyRate));
    Compensation = compensation;
  }

  public void UpdateGender(GenderType gender)
  {
    Guard.Against.Null(gender, nameof(gender));
    Gender = gender;
  }

  public void UpdateBiography(string? biography)
  {
    Biography = biography;
  }

  public void UpdateDateOfBirth(DateTime dateOfBirth)
  {
    DateOfBirth = dateOfBirth;
  }

  public void UpdateIsActive(bool isActive)
  {
    IsActive = isActive;
  }

  public void UpdateIsVerified(bool isVerified)
  {
    IsVerified = isVerified;
  }

  public void UpdateYearsOfExperience(int yearsOfExperience)
  {
    Guard.Against.Negative(yearsOfExperience, nameof(yearsOfExperience));
    YearsOfExperience = yearsOfExperience;
  }

  public void UpdateHasInsurance(bool hasInsurance)
  {
    HasInsurance = hasInsurance;
  }

  public void UpdateHasFirstAidCertification(bool hasFirstAidCertification)
  {
    HasFirstAidCertificatation = hasFirstAidCertification;
  }

  public void UpdateDailyPetWalkLimit(int dailyPetWalkLimit)
  {
    Guard.Against.Negative(dailyPetWalkLimit, nameof(dailyPetWalkLimit));
    DailyPetWalkLimit = dailyPetWalkLimit;
  }
}

