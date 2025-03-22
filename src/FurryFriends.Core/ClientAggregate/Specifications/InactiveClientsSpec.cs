using FurryFriends.Core.ClientAggregate;

public sealed class InactiveClientsSpec : Specification<Client>
{
    public InactiveClientsSpec()
    {
        Query
            .Where(x => !x.IsActive)
            .Include(x => x.Pets)
            .OrderByDescending(x => x.DeactivatedAt);
    }
}