using FurryFriends.Core.Interfaces;
using FurryFriends.Core.UserAggregate;


public class ExtendedUserMetadata : ValueObject
{
    public User User { get; }
    public string PetWalkerId { get; }
    public string ClientId { get; }

    public ExtendedUserMetadata(User user, string petWalkerId, string clientId)
    {
        this.User = user;
        PetWalkerId = petWalkerId;
        ClientId = clientId;
    }

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return User;
    yield return PetWalkerId;
    yield return ClientId;

  }
}