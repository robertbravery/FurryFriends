using FurryFriends.UseCases.Domain.Clients.DTO;
using FurryFriends.UseCases.Services.ClientService;

namespace FurryFriends.UseCases.Domain.Clients.Query.GetClient;
public class GetClientQueryHandler(IClientService clientService)
  : IQueryHandler<GetClientQuery, Result<ClientDTO>>
{
  private readonly IClientService _clientService = clientService;

  public async Task<Result<ClientDTO>> Handle(GetClientQuery request, CancellationToken cancellationToken)
  {
    var entityResult = await _clientService.GetClientAsync(request.EmailAddress, cancellationToken);

    if (!entityResult.IsSuccess)
    {
      return Result.NotFound("Client not found");
    }

    return new ClientDTO(
      entityResult.Value.Id,
      entityResult.Value.Name.FullName,
      entityResult.Value.Email.EmailAddress,
      entityResult.Value.PhoneNumber.CountryCode,
      entityResult.Value.PhoneNumber.Number,
      entityResult.Value.Address.Street,
      entityResult.Value.Address.City,
      entityResult.Value.Address.StateProvinceRegion,
      entityResult.Value.Address.ZipCode,
      entityResult.Value.Address.Country,
      entityResult.Value.ClientType,
      entityResult.Value.PreferredContactTime,
      entityResult.Value.ReferralSource,
      [..  entityResult.Value.Pets.Select(p => new ClientPetDto(
            p.Id,
            p.Name,
            p.BreedType.Species.Name,
            p.BreedType.Name,
            p.Age,
            p.Weight,
            p.IsSterilized,
            p.IsVaccinated,
            p.SpecialNeeds,
            p.MedicalConditions,
            p.IsActive
        ))]
      );
  }
}
