namespace FurryFriends.Core.ClientAggregate.Specifications;

public sealed class ListClientsSpec : Specification<Client>
{
  public ListClientsSpec(
    string? searchTerm, 
    int page, 
    int pageSize, 
    bool includeInactive = false,
    bool isAsNoTracking = true)
  {
    if (!includeInactive)
    {
      Query.Where(x => x.IsActive);
    }

    if (!string.IsNullOrEmpty(searchTerm))
    {
      Query.Where(x => x.Name.FirstName.Contains(searchTerm)
        || x.Name.LastName.Contains(searchTerm)
        || x.Email.EmailAddress.Contains(searchTerm));
    }

    Query.Skip((page - 1) * pageSize)
      .Take(pageSize)
      .OrderBy(o => o.Name.FirstName)
      .Include(x => x.Address)
      .Include(x => x.Name)
      .Include(x => x.Pets.Where(p => includeInactive || p.IsActive))
        .ThenInclude(p => p.BreedType)
          .ThenInclude(b => b.Species);
    
    if (isAsNoTracking) Query.AsNoTracking();
  }
}
