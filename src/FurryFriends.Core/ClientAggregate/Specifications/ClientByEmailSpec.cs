namespace FurryFriends.Core.ClientAggregate.Specifications;
public sealed class ClientByEmailSpec : SingleResultSpecification<Client>
{
  public ClientByEmailSpec(string emailAddress)
  {
    Query.Where(x => x.Email.EmailAddress == emailAddress);
  }
}
