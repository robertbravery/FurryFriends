namespace FurryFriends.Core.ClientAggregate.Specifications;

public sealed class ListClientsSpec : Specification<Client>
{
  public ListClientsSpec(string? searchTerm, int page, int pageSize, bool isAsNoTracking = true)
  {
    if (!string.IsNullOrEmpty(searchTerm))
    {
      Query.Where(x => x.Name.FirstName.Contains(searchTerm)
      || x.Name.LastName.Contains(searchTerm)
      || x.Email.EmailAddress.Contains(searchTerm));
    }

    Query.Skip((page - 1) * pageSize).Take(pageSize)
      .OrderBy(o => o.Name.FirstName)
      .Include(x => x.Address)
      .Include(x => x.Name);
    if (isAsNoTracking) Query.AsNoTracking();
  }
}
