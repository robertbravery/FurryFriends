using FurryFriends.Core.Entities;

namespace FurryFriends.Core.UserAggregate.Specifications;

public class GetUserByEmailSpecification : SingleResultSpecification<User>
{
  public GetUserByEmailSpecification(string email) =>
    Query
        .Where(w => w.Email == email);
}
