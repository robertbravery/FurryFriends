using FurryFriends.Core.ValueObjects;

namespace FurryFriends.Core.UserAggregate.Specifications;

public class GetUserByEmailSpecification : SingleResultSpecification<User>
{
  public GetUserByEmailSpecification(string email, int page = 1, int pageSize = 10) =>
    Query
        .Where(w => w.Email.EmailAddress == email)
        .Include(i => i.Photos)
        .Include(i => i.ServiceAreas).ThenInclude(i => i.Locality)
        .ThenInclude(i => i.Region)
        .ThenInclude(i => i.Country);
}
