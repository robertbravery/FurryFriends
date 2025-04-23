using FurryFriends.UseCases.Domain.Clients.DTO;
using FurryFriends.Web.Endpoints.ClientEndpoints.Records;

namespace FurryFriends.Web.Endpoints.ClientEndpoints.Get;

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
        e.Country,
        e.ClientType,
        e.PreferredContactTime,
        e.ReferralSource,
        [.. e.Pets.Select(p => new PetRecord(
            p.Id,
            p.Name,
            "Species", //p.Species,
            p.Breed,
            p.Age,
            p.Weight,
            p.SpecialNeeds,
            p.MedicalConditions,
            p.IsActive
        ))]
    );
  }
}

