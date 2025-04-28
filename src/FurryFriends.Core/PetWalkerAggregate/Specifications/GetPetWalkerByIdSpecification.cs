namespace FurryFriends.Core.PetWalkerAggregate.Specifications;

public class GetPetWalkerByIdSpecification : SingleResultSpecification<PetWalker>
{
  public GetPetWalkerByIdSpecification(Guid id, bool isAsNoTracking = false)
  {
    Query.Where(x => x.Id == id);
    Query.Include(x => x.ServiceAreas);

    if (isAsNoTracking)
    {
      Query.AsNoTracking();
    }
  }
}
