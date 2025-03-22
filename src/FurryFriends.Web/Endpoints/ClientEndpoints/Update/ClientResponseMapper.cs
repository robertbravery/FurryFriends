using FurryFriends.Core.ClientAggregate;

namespace FurryFriends.Web.Endpoints.ClientEndpoints.Update;

public class ClientResponseMapper : Mapper<UpdateClientRequest, UpdateClientResponse, Client>
{
  public override UpdateClientResponse FromEntity(Client client)
  {
    return new UpdateClientResponse
    {
      ClientId = client.Id,
      FirstName = client.Name.FirstName,
      LastName = client.Name.LastName,
      Email = client.Email.EmailAddress,
      PhoneNumber = client.PhoneNumber.Number,
      Address = client.Address.ToString(),
      ClientType = client.ClientType,
      PreferredContactTime = client.PreferredContactTime,
      ReferralSource = client.ReferralSource
    };
  }
}
