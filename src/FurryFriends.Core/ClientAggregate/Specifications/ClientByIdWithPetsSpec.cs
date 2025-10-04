namespace FurryFriends.Core.ClientAggregate.Specifications;
public sealed class ClientByIdWithPetsSpec : SingleResultSpecification<Client>
{
  public ClientByIdWithPetsSpec(Guid clientId)
  {
    Query
      .Where(x => x.Id == clientId)
      .Include(x => x.Pets.Where(p => p.IsActive))
      .AsTracking();  // Explicitly enable tracking
  }
}

