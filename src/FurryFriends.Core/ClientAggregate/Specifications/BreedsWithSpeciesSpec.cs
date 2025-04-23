namespace FurryFriends.Core.ClientAggregate.Specifications;

// Specification to include the Species navigation property
public class BreedsWithSpeciesSpec : Specification<Breed>
{
  public BreedsWithSpeciesSpec()
  {
    Query.Include(b => b.Species);
  }
}

