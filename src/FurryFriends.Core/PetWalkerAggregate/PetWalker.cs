using FurryFriends.Core.Common;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.Core.PetWalkerAggregate;

public class PetWalker : UserEntityBase
{
  public const int DefaultDailyPetWalkLimit = 8;
  public const int DefaultSlotDurationMinutes = 60;
  public const int DefaultInterBookingGapMinutes = 0;

  private readonly List<Schedule> _schedules = new();

  public GenderType Gender { get; private set; } = GenderType.Create(GenderType.GenderCategory.Other).Value;
  public string? Biography { get; private set; }
  public DateTime DateOfBirth { get; private set; }
  public Compensation Compensation { get; private set; } = default!;
  public bool IsActive { get; private set; }
  public bool IsVerified { get; private set; }
  public int YearsOfExperience { get; private set; }
  public bool HasInsurance { get; private set; }
  public bool HasFirstAidCertificatation { get; private set; }

  /// <summary>
  /// Maximum number of walks per day. Defaults to 8 rather than 0: the daily-limit
  /// guard rejects when bookingsOnDate &gt;= limit, so a default of 0 would make a
  /// walker who never configured a limit unbookable.
  /// </summary>
  public int DailyPetWalkLimit { get; private set; } = DefaultDailyPetWalkLimit;

  /// <summary>Length of a derived bookable slot, in minutes.</summary>
  public int SlotDurationMinutes { get; private set; } = DefaultSlotDurationMinutes;

  /// <summary>Required gap between consecutive bookings, in minutes.</summary>
  public int InterBookingGapMinutes { get; private set; }

  public double? AverageRating { get; private set; }
  public int TotalRatingsCount { get; private set; }
  public IReadOnlyCollection<Schedule> Schedules => _schedules.AsReadOnly();

  /// <summary>
  /// True when the walker has published at least one weekly schedule. Used to tell
  /// "this walker has not set availability at all" apart from "this day is full".
  /// </summary>
  public bool HasAnySchedule => _schedules.Count > 0;

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

  public void UpdateRatingAggregate(double? averageRating, int totalRatingsCount)
  {
    AverageRating = averageRating;
    TotalRatingsCount = totalRatingsCount;
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

  public void UpdateServiceArea(ServiceArea serviceArea)
  {
    Guard.Against.Null(serviceArea, nameof(serviceArea));
    ServiceAreas.Add(serviceArea);
  }

  public void AddServiceArea(ServiceArea serviceArea)
  {
    Guard.Against.Null(serviceArea, nameof(serviceArea));
    ServiceAreas.Add(serviceArea);
  }

  public void RemoveServiceArea(ServiceArea serviceArea)
  {
    Guard.Against.Null(serviceArea, nameof(serviceArea));
    ServiceAreas.Remove(serviceArea);
  }

  public void AddSchedule(Schedule schedule)
  {
    // Optional: enforce "no duplicate day" rule
    if (_schedules.Any(s => s.DayOfWeek == schedule.DayOfWeek &&
                            s.StartTime == schedule.StartTime &&
                            s.EndTime == schedule.EndTime))
    {
      throw new InvalidOperationException("Schedule already exists.");
    }
    _schedules.Add(schedule);
  }

  public void RemoveSchedule(Schedule schedule)
  {
    _schedules.Remove(schedule);
  }

  public void ClearSchedules() => _schedules.Clear();

  /// <summary>
  /// Sets the derived slot length. Rejects zero or negative values so a bad write
  /// cannot silently make the walker unbookable - slot generation would yield nothing.
  /// </summary>
  public void UpdateSlotDurationMinutes(int slotDurationMinutes)
  {
    Guard.Against.OutOfRange(slotDurationMinutes, nameof(slotDurationMinutes), 15, 480);
    SlotDurationMinutes = slotDurationMinutes;
  }

  public void UpdateInterBookingGapMinutes(int interBookingGapMinutes)
  {
    Guard.Against.OutOfRange(interBookingGapMinutes, nameof(interBookingGapMinutes), 0, 240);
    InterBookingGapMinutes = interBookingGapMinutes;
  }

}