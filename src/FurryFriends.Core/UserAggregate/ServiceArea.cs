using FurryFriends.Core.Common;
using FurryFriends.Core.LocationAggregate;

namespace FurryFriends.Core.UserAggregate;
public class ServiceArea : AuditableEntity<Guid>, IEquatable<ServiceArea>
{
    public Guid UserID { get; private set; }
    public Guid LocalityID { get; private set; }

    public virtual User User { get; private set; } = null!;
    public virtual Locality Locality { get; private set; } = null!;


    private ServiceArea(Guid userID,  Guid localityID)
    {
        UserID = userID;
        LocalityID = localityID;
    }

  public static ServiceArea Create(Guid userID,  Guid localityID)
  {
    Guard.Against.Default(userID, nameof(userID));
    Guard.Against.Default(localityID, nameof(localityID));

    return new ServiceArea(userID,  localityID);
  }

    public bool Equals(ServiceArea? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        return obj is ServiceArea serviceArea && Equals(serviceArea);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}

