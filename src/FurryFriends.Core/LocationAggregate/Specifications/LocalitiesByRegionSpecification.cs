using FurryFriends.Core.Common;
using FurryFriends.Core.LocationAggregate;

namespace FurryFriends.Core.LocationAggregate.Specifications;

public class LocalitiesByRegionSpecification : Specification<Locality>
{
  public LocalitiesByRegionSpecification(Guid regionId)
  {
    Query.Where(l => l.RegionID == regionId);
  }
}
