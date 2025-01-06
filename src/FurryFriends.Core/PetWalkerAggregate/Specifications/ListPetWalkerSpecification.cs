namespace FurryFriends.Core.PetWalkerAggregate.Specifications;
public class ListPetWalkerSpecification : Specification<PetWalker>
{
  public ListPetWalkerSpecification(string? searchString, int? pageNumber = 1, int? pageSize = 10)
  {
    Query
      .OrderBy(x => x.Name.FirstName);

    if (!string.IsNullOrEmpty(searchString))
    {
      Query.Where(x => x.Name.FirstName.Contains(searchString)
      || x.Name.LastName.Contains(searchString)
      || x.Email.EmailAddress.Contains(searchString));
    }

    if (pageSize.HasValue && pageNumber.HasValue)
    {
      Query
        .Skip((pageNumber.Value - 1) * pageSize.Value)
        .Take(pageSize.Value);
    }
  }

}
