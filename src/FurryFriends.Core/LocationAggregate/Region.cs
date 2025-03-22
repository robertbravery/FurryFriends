using FurryFriends.Core.Common;

namespace FurryFriends.Core.LocationAggregate;

public class Region(string regionName, Guid countryID) : AuditableEntity<Guid>
{
  public string RegionName { get; set; } = regionName ?? throw new ArgumentNullException(nameof(regionName));
  public Guid CountryID { get; set; } = countryID;
  public virtual Country Country { get; set; } = default!;
  public virtual ICollection<Locality> Localities { get; set; } = default!;


}
