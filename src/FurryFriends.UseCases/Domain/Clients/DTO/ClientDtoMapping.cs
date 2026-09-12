using FurryFriends.Core.ClientAggregate;

namespace FurryFriends.UseCases.Domain.Clients.DTO;

/// <summary>
/// Single projection from the <see cref="Client"/> aggregate to <see cref="ClientDTO"/>.
/// Kept in one place so the lookup-by-email and lookup-by-id queries cannot drift apart.
/// </summary>
public static class ClientDtoMapping
{
  public static ClientDTO ToDto(this Client client) => new(
    client.Id,
    client.Name.FullName,
    client.Email.EmailAddress,
    client.PhoneNumber.CountryCode,
    client.PhoneNumber.Number,
    client.Address.Street,
    client.Address.City,
    client.Address.StateProvinceRegion,
    client.Address.ZipCode,
    client.Address.Country,
    client.ClientType,
    client.PreferredContactTime,
    client.ReferralSource,
    [.. client.Pets.Select(p => new ClientPetDto(
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
      p.IsActive))]);
}
