using FurryFriends.Core.ValueObjects;

namespace FurryFriends.Core.UserAggregate.Specifications;

public class GetUserByEmailSpecification : SingleResultSpecification<User>
{
  public GetUserByEmailSpecification(string email) =>
    Query
        .Where(w => w.Email.EmailAddress == email)
    .Include(i=> i.Photos);
}
