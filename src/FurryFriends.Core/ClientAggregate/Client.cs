using FurryFriends.Core.ClientAggregate.Enums;
using FurryFriends.Core.Common;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.Core.ClientAggregate;


public class Client : UserEntityBase, IAggregateRoot
{
  public virtual List<Pet> Pets { get; private set; } = default!;

  public ClientType ClientType { get; private set; }
  public TimeOnly? PreferredContactTime { get; private set; }
  public ReferralSource ReferralSource { get; private set; }



  internal Client() { }

  private Client(
        Name Name,
        Email Email,
        PhoneNumber PhoneNumber,
        Address Address,
        ClientType ClientType = ClientType.Regular,
        TimeOnly? PreferredContactTime = null,
        ReferralSource ReferralSource = ReferralSource.None)
    : base(Name, Email, PhoneNumber, Address)
  {
    this.Name = Name;
    this.Email = Email;
    this.PhoneNumber = PhoneNumber;
    this.Address = Address;
    this.ClientType = ClientType;
    this.PreferredContactTime = PreferredContactTime;
    this.ReferralSource = ReferralSource;
  }

  public static Client Create(
    Name name,
    Email email,
    PhoneNumber phoneNumber,
    Address address,
    ClientType clientType = ClientType.Regular,
    ReferralSource referralSource = ReferralSource.None,
    TimeOnly? preferredContactTime = null
    )
  {
    Guard.Against.Null(name, nameof(name));
    Guard.Against.Null(email, nameof(email));
    Guard.Against.Null(address, nameof(address));
    Guard.Against.Null(phoneNumber, nameof(phoneNumber));
    Guard.Against.EnumOutOfRange(clientType, nameof(clientType));


    return new Client(name, email, phoneNumber, address, clientType, preferredContactTime, referralSource);
  }

  public Result<Pet> AddPet(string name, int breedId, int age,
      double weight, string color, string specialNeeds, string? dietaryRestrictions = null)
  {
    Guard.Against.NullOrWhiteSpace(name, nameof(name));

    if (HasDuplicatePet(name, breedId))
      return Result.Error("A pet with the same name, species, and breed already exists for this client");

    if (HasReachedPetLimit())
      return Result.Error("Maximum number of pets reached");

    var pet = Pet.Create(name, breedId, age, weight, color, specialNeeds, this);

    Pets ??= [];
    Pets.Add(pet);

    return Result.Success(pet);
  }

  public Result UpdatePetInfo(Guid petId, string name, int age, double weight,
      string color, string? dietaryRestrictions, string? favoriteActivities)
  {
    var pet = Pets.FirstOrDefault(p => p.Id == petId);
    if (pet == null)
      return Result.Error("Pet not found in this client's pets");

    pet.UpdateGeneralInfo(name, age, weight, color, dietaryRestrictions, favoriteActivities);
    return Result.Success();
  }

  public Result RemovePet(Guid petId)
  {
    var pet = Pets.FirstOrDefault(p => p.Id == petId);
    if (pet == null)
      return Result.Error("Pet not found in this client's pets");

    pet.MarkAsInactive();
    return Result.Success();
  }

  public bool HasReachedPetLimit()
  {
    const int MaxPetsPerClient = 5;
    return Pets.Count(p => p.IsActive) >= MaxPetsPerClient;
  }

  public void UpdateClientType(ClientType clientType)
  {
    Guard.Against.EnumOutOfRange(clientType, nameof(clientType));
    ClientType = clientType;
  }

  public void UpdatePreferredContactTime(TimeOnly? preferredContactTime)
  {
    PreferredContactTime = preferredContactTime;
  }

  public void UpdateReferralSource(ReferralSource referralSource)
  {
    // if (referralSource.HasValue)
    // {
    //   Guard.Against.EnumOutOfRange(referralSource.Value, nameof(referralSource));
    // }
    ReferralSource = referralSource;
  }

  private bool HasDuplicatePet(string name, int breedId)
  {
    return Pets?.Any(p =>
        p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
        p.BreedId == breedId &&
        p.IsActive) ?? false;
  }

}
