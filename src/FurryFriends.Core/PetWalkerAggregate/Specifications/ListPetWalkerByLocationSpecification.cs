namespace FurryFriends.Core.PetWalkerAggregate.Specifications;

public class ListPetWalkerByLocationSpecification : Specification<PetWalker>
{
  public ListPetWalkerByLocationSpecification(string? searchString, Guid? localityId, int page = 1, int pageSize = 10)
  {
    if (localityId.HasValue)
    {
      Query.Where(user => user.ServiceAreas.Any(sa => sa.LocalityID == localityId.Value));
    }

    if (!string.IsNullOrEmpty(searchString))
    {
      Query.Where(user => user.Name.FirstName.Contains(searchString)
      || user.Name.LastName.Contains(searchString)
      || user.Email.EmailAddress.Contains(searchString));
    }

    Query.OrderBy(x => x.Name.FirstName)
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .Include(i => i.ServiceAreas.Where(sa => sa.LocalityID == localityId)).ThenInclude(i => i.Locality);
  }
}
