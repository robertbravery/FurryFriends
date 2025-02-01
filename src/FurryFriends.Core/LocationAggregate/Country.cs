using FurryFriends.Core.Common;

namespace FurryFriends.Core.LocationAggregate;

public class Country(string countryName) : AuditableEntity<Guid>, IAggregateRoot
{

  public string CountryName { get; set; } = countryName;
  public virtual ICollection<Region> Regions { get; set; } = [];
}
