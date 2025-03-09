using FurryFriends.UseCases.Domain.Clients.Query;

namespace FurryFriends.Web.Endpoints.ClientEnpoints.Get;

public class ClientDTOMapper : Mapper<GetClientRequest, ClientRecord, ClientDTO>
{
  public override ClientRecord FromEntity(ClientDTO e)
  {
    return new ClientRecord(
        e.Id,
        e.Name,
        e.Email,
        e.PhoneNumber,
        e.Street,
        e.City,
        e.State,
        e.ZipCode,
        e.ClientType,
        e.PreferredContactTime,
        e.ReferralSource
    );
  }
}

