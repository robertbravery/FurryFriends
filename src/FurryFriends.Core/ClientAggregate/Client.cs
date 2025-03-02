using FurryFriends.Core.ClientAggregate.Enums;
using FurryFriends.Core.Common;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.Core.ClientAggregate;


public class Client : UserEntityBase, IAggregateRoot
{
  public virtual ICollection<Pet> Pets { get; set; } = new HashSet<Pet>();

  public ClientType ClientType { get; private set; }
  public TimeOnly? PreferredContactTime { get; private set; }
  public string? ReferralSource { get; private set; }



  internal Client() { }

  private Client(
        Name Name,
        Email Email,
        PhoneNumber PhoneNumber,
        Address Address,
        ClientType ClientType = ClientType.Regular,
        TimeOnly? PreferredContactTime = null,
        string? ReferralSource = null)
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
    TimeOnly? preferredContactTime = null,
    string? referralSource = null)
  {
    Guard.Against.Null(name, nameof(name));
    Guard.Against.Null(email, nameof(email));
    Guard.Against.Null(address, nameof(address));
    Guard.Against.Null(phoneNumber, nameof(phoneNumber));
    Guard.Against.EnumOutOfRange(clientType, nameof(clientType));

    return new Client(name, email, phoneNumber, address, clientType, preferredContactTime, referralSource);
  }

  public void AddPet(Pet pet)
  {
    Guard.Against.Null(pet, nameof(pet));
    Pets.Add(pet);
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

  public void UpdateReferralSource(string? referralSource)
  {
    ReferralSource = referralSource;
  }


}

