using FurryFriends.Core.Common;

namespace FurryFriends.Core.LocationAggregate;

public class Country : AuditableEntity<Guid>, IAggregateRoot
{
  public string CountryName { get; set; } = default!;
  public virtual ICollection<Region> Regions { get; set; } = default!;
}
