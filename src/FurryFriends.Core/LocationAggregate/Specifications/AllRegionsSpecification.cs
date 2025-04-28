using FurryFriends.Core.Common;
using FurryFriends.Core.LocationAggregate;

namespace FurryFriends.Core.LocationAggregate.Specifications;

public class AllRegionsSpecification : Specification<Region>
{
  public AllRegionsSpecification()
  {
    Query.OrderBy(r => r.RegionName);
  }
}
