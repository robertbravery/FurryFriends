using FurryFriends.Core.Entities;

namespace FurryFriends.Core.UserAggregate.Specifications;

public class GetUserByEmailSpec : SingleResultSpecification<User>
{
  public GetUserByEmailSpec(string email) =>
    Query
        .Where(w => w.Email == email);
}
