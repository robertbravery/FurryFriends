namespace FurryFriends.Core.ClientAggregate.Specifications;

/// <summary>
/// Read-only projection of a client for lookups by id. Pets are eager-loaded with their
/// breed and species because <c>ClientDTO</c> exposes both, and lazy loading is not enabled.
/// Only active pets are returned, matching <see cref="ClientByEmailSpec"/>.
/// </summary>
public sealed class ClientByIdWithDetailsSpec : SingleResultSpecification<Client>
{
  public ClientByIdWithDetailsSpec(Guid clientId)
  {
    Query
      .Where(x => x.Id == clientId)
      .Include(x => x.Pets.Where(p => p.IsActive))
        .ThenInclude(p => p.BreedType)
        .ThenInclude(b => b.Species)
      .AsNoTracking();
  }
}
