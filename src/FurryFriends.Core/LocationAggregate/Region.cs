using FurryFriends.Core.Common;

namespace FurryFriends.Core.LocationAggregate;

public class Region : AuditableEntity<Guid>
{
  public string RegionName { get; set; } = default!;
  public Guid CountryID { get; set; }
  public virtual Country Country { get; set; } = default!;
  public virtual ICollection<Locality> Localities { get; set; } = default!;
}
