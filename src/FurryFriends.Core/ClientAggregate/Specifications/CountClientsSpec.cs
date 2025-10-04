namespace FurryFriends.Core.ClientAggregate.Specifications;

public sealed class CountClientsSpec : Specification<Client>
{
  public CountClientsSpec(
    string? searchTerm, 
    bool includeInactive = false)
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
    
    Query.AsNoTracking();
  }
}
