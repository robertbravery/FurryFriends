using FurryFriends.Core.UserAggregate;

namespace FurryFriends.Core.PetWalkerAggregate.Specifications;

public class GetPetWalkerByEmailSpecification : SingleResultSpecification<PetWalker>
{
  public GetPetWalkerByEmailSpecification(string email, int page = 1, int pageSize = 10) =>
    Query
        .Where(w => w.Email.EmailAddress == email)
        .Include(i => i.Photos)
        .Include(i => i.ServiceAreas).ThenInclude(i => i.Locality)
        .ThenInclude(i => i.Region)
        .ThenInclude(i => i.Country);
}
