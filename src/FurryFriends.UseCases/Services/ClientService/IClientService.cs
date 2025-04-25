using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.ClientAggregate.Enums;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Domain.Clients.Query.ListClients;

namespace FurryFriends.UseCases.Services.ClientService;

public interface IClientService
{
  Task<Result<Guid>> AddPetToClientAsync(
      Guid clientId,
      string name,
      int breedId,
      int age,
      double weight,
      string color,
      string specialNeeds,
      string? dietaryRestrictions,
      CancellationToken cancellationToken);

  Task<Result> AddPetMedicalConditionAsync(
      Guid clientId,
      Guid petId,
      string medicalCondition,
      CancellationToken cancellationToken);

  Task<Result> UpdatePetVaccinationStatusAsync(
      Guid clientId,
      Guid petId,
      bool isVaccinated,
      CancellationToken cancellationToken);

  Task<Result> UpdatePetFavoriteActivitiesAsync(
      Guid clientId,
      Guid petId,
      string favoriteActivities,
      CancellationToken cancellationToken);

  Task<Result> UpdatePetDietaryRestrictionsAsync(
      Guid clientId,
      Guid petId,
      string dietaryRestrictions,
      CancellationToken cancellationToken);

  Task<Result> UpdatePetSpecialNeedsAsync(
      Guid clientId,
      Guid petId,
      string specialNeeds,
      CancellationToken cancellationToken);

  Task<Result> UpdatePetSterilizationStatusAsync(
      Guid clientId,
      Guid petId,
      bool isSterilized,
      CancellationToken cancellationToken);

  Task<Result<Client>> CreateClientAsync(
    Name name,
    Email email,
    PhoneNumber phoneNumber,
    Address address,
    ClientType clientType = ClientType.Regular,
    ReferralSource referralSource = ReferralSource.None,
    CancellationToken cancellationToken = default);

  Task<Result<Client>> GetClientAsync(string emailAddress, CancellationToken cancellationToken);
  Task<Result<ClientListDto>> ListClientsAsync(ListClientQuery query, CancellationToken cancellationToken);
  Task<Result<int>> CountClientsAsync(string? searchTerm, CancellationToken cancellationToken);
  Task<Result<Client>> UpdateClientAsync(Client client);
}

