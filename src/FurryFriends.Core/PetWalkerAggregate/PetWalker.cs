using FurryFriends.Core.Common;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.Core.PetWalkerAggregate;

public class PetWalker : UserEntityBase
{
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

  private PetWalker() { }

  private PetWalker(Name name, Email email, PhoneNumber phoneNumber, Address address)
  {
    Id = Guid.NewGuid();
    Name = name;
    Email = email;
    PhoneNumber = phoneNumber;
    Address = address;
  }

  public static PetWalker Create(Name name, Email email, PhoneNumber phoneNumber, Address address)
  {
    Guard.Against.Null(name, nameof(name));
    Guard.Against.Null(email, nameof(email));
    Guard.Against.Null(address, nameof(address));
    return new PetWalker(name, email, phoneNumber, address);
  }

  public void Deactivate()
  {
    IsActive = false;
  }

  public void UpdateUsername(Name newUserName)
  {
    Guard.Against.NullOrEmpty(newUserName.FirstName, nameof(newUserName.FirstName));
    Guard.Against.StringTooLong(newUserName.FirstName, 30, nameof(newUserName.FirstName));
    Guard.Against.StringTooShort(newUserName.LastName, 5, nameof(newUserName.LastName)); Guard.Against.NullOrEmpty(newUserName.FirstName, nameof(newUserName.FirstName));
    Guard.Against.StringTooLong(newUserName.LastName, 30, nameof(newUserName.LastName));
    Guard.Against.StringTooShort(newUserName.LastName, 5, nameof(newUserName.LastName));
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

  public void UpdateAddress(string street, string city, string state, string zipCode, string country)
  {
    Guard.Against.NullOrEmpty(street, nameof(street));
    Guard.Against.NullOrEmpty(city, nameof(city));
    Guard.Against.NullOrEmpty(state, nameof(state));
    Guard.Against.NullOrEmpty(zipCode, nameof(zipCode));
    Guard.Against.NullOrEmpty(country, nameof(country));
    Address = Address.Create(street, city, state, zipCode, country);
  }
  public void UpdateAddress(Address address)
  {
    Guard.Against.Null(address, nameof(address));
    Address = address;
  }

  public void UpdatePhoneNumber(string countryCode, string phoneNumber)
  {
    Guard.Against.NullOrEmpty(phoneNumber, nameof(phoneNumber));
    Guard.Against.NullOrEmpty(countryCode, nameof(countryCode));
    PhoneNumber = PhoneNumber.Create(countryCode, phoneNumber).GetAwaiter().GetResult();
  }
  public void UpdatePhoneNumber(PhoneNumber phoneNumber)
  {
    Guard.Against.Null(phoneNumber, nameof(phoneNumber));
    PhoneNumber = phoneNumber;
  }
}

