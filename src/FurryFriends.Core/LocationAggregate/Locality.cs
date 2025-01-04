using FurryFriends.Core.Common;

namespace FurryFriends.Core.LocationAggregate;

public class Locality : AuditableEntity<Guid>
{
  public string LocalityName { get; set; } = default!;
  public Guid RegionID { get; set; }
  public virtual Region Region { get; set; } = default!;
}
