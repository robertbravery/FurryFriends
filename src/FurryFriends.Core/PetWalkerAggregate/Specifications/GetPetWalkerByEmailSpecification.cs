namespace FurryFriends.Core.PetWalkerAggregate.Specifications;

public class GetPetWalkerByEmailSpecification : SingleResultSpecification<PetWalker>
{
  public GetPetWalkerByEmailSpecification(string email) =>
    Query
        .Where(w => w.Email.EmailAddress == email)
        .Include(i => i.Photos)
        .Include(i => i.ServiceAreas).ThenInclude(i => i.Locality)
        .ThenInclude(i => i.Region)
        .ThenInclude(i => i.Country);
}
