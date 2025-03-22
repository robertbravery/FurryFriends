using FurryFriends.Core.Common;

namespace FurryFriends.Core.LocationAggregate;

public class Locality(string localityName, Guid regionID) : AuditableEntity<Guid>
{
  public string LocalityName { get; set; } = localityName;
  public Guid RegionID { get; set; } = regionID;
  public virtual Region Region { get; set; } = default!;
}
