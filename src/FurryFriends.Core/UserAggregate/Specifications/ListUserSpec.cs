using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FurryFriends.Core.Entities;

namespace FurryFriends.Core.UserAggregate.Specifications;
public class ListUserSpec : Specification<User>
{
  public ListUserSpec(string? searchString, int? pageSize, int? pageNumber)
  {
    Query
      .OrderBy(x => x.Name);

    if (!string.IsNullOrEmpty(searchString))
    {
      Query.Where(x => x.Name.Contains(searchString));
    }

    if (pageSize.HasValue && pageNumber.HasValue)
    {
      Query
        .Skip((pageNumber.Value - 1) * pageSize.Value)
        .Take(pageSize.Value);
    }
  }

}
