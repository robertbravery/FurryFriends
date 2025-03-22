namespace FurryFriends.Core.ClientAggregate.Specifications;
public sealed class ClientByEmailSpec : SingleResultSpecification<Client>
{
  public ClientByEmailSpec(string emailAddress, bool isAsNoTracking = false)
  {
    Query.Where(x => x.Email.EmailAddress == emailAddress);
    if (isAsNoTracking) Query.AsNoTracking();
  }
}
