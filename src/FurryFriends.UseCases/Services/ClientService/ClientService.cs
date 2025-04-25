using Ardalis.GuardClauses;
using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.ClientAggregate.Enums;
using FurryFriends.Core.ClientAggregate.Specifications;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Domain.Clients.Query.ListClients;

namespace FurryFriends.UseCases.Services.ClientService;

public class ClientService(IRepository<Client> repository) : IClientService
{
  private readonly IRepository<Client> _repository = repository;

  public async Task<Result<Client>> CreateClientAsync(
    Name name,
    Email email,
    PhoneNumber phoneNumber,
    Address address,
    ClientType clientType = ClientType.Regular,
    ReferralSource referralSource = ReferralSource.None,
    CancellationToken cancellationToken = default)
  {
    var existingClientSpec = new ClientByEmailSpec(email.EmailAddress);
    if (await _repository.AnyAsync(existingClientSpec, cancellationToken))
    {
      return Result.Error("A client with this email already exists");
    }

    var client = Client.Create(name, email, phoneNumber, address, clientType, referralSource);

    await _repository.AddAsync(client, cancellationToken);
    await _repository.SaveChangesAsync(cancellationToken);

    return Result.Success(client);
  }

  public async Task<Result<Client>> UpdateClientAsync(Client client)
  {
    var updatedClient = await _repository.GetByIdAsync(client.Id)
      ?? throw new NotFoundException(nameof(Client), client.Id.ToString());
    updatedClient.UpdateDetails(
        client.Name,
        client.Email,
        client.PhoneNumber,
        client.Address
    );

    client.UpdateClientType(client.ClientType);
    client.UpdatePreferredContactTime(client.PreferredContactTime);
    client.UpdateReferralSource(client.ReferralSource);

    await _repository.UpdateAsync(client);

    return client;
  }

  public async Task<Result<Client>> GetClientAsync(string emailAddress, CancellationToken cancellationToken)
  {
    var existingClientSpec = new ClientByEmailSpec(emailAddress, true);
    var client = await _repository.FirstOrDefaultAsync(existingClientSpec, cancellationToken);
    if (client == null)
    {
      return Result.Error("Client not found");
    }
    return Result.Success(client);

  }

  public async Task<Result<ClientListDto>> ListClientsAsync(ListClientQuery query, CancellationToken cancellationToken)
  {
    var listSpec = new ListClientsSpec(query.SearchTerm, query.Page, query.PageSize);
    var clients = await _repository.ListAsync(listSpec, cancellationToken);
    var totalCount = await _repository.CountAsync(listSpec, cancellationToken);
    if (clients == null)
    {
      return Result.Error("No Clients found");
    }
    return new ClientListDto(clients, totalCount);
  }

  public async Task<Result<int>> CountClientsAsync(string? searchTerm, CancellationToken cancellationToken)
  {
    var countSpec = new CountClientsSpec(searchTerm);
    int count = await _repository.CountAsync(countSpec, cancellationToken);
    return Result.Success(count);
  }

  // Add these methods to the existing ClientService implementation
  public async Task<Result<Guid>> AddPetToClientAsync(
    Guid clientId,
    string name,
    int breedId,
    int age,
    double weight,
    string color,
    string specialNeeds,
    string? dietaryRestrictions,
    CancellationToken cancellationToken)
  {
    var spec = new ClientByIdWithPetsSpec(clientId);
    var client = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (client == null)
    {
      return Result.NotFound("Client not found");
    }

    var petResult = client.AddPet(name, breedId, age, weight, color, specialNeeds, dietaryRestrictions);
    if (!petResult.IsSuccess)
    {
      return Result.Error(new ErrorList(petResult.Errors)); // Forward the original error messages
    }

    try
    {
      await _repository.SaveChangesAsync(cancellationToken);
      return Result.Success(petResult.Value.Id);
    }
    catch (Exception ex)
    {
      throw new Exception("Error adding pets to client", ex);
    }
  }

  public async Task<Result> AddPetMedicalConditionAsync(
      Guid clientId,
      Guid petId,
      string medicalCondition,
      CancellationToken cancellationToken)
  {
    var client = await _repository.GetByIdAsync(clientId, cancellationToken);
    if (client == null)
    {
      return Result.NotFound("Client not found");
    }

    var pet = client.Pets.FirstOrDefault(p => p.Id == petId);
    if (pet == null)
    {
      return Result.NotFound("Pet not found");
    }

    pet.AddMedicalCondition(medicalCondition);
    await _repository.SaveChangesAsync(cancellationToken);

    return Result.Success();
  }

  public async Task<Result> UpdatePetVaccinationStatusAsync(
      Guid clientId,
      Guid petId,
      bool isVaccinated,
      CancellationToken cancellationToken)
  {
    var client = await _repository.GetByIdAsync(clientId, cancellationToken);
    if (client == null)
    {
      return Result.NotFound("Client not found");
    }

    var pet = client.Pets.FirstOrDefault(p => p.Id == petId);
    if (pet == null)
    {
      return Result.NotFound("Pet not found");
    }

    pet.UpdateVaccinationStatus(isVaccinated);
    await _repository.SaveChangesAsync(cancellationToken);

    return Result.Success();
  }

  public async Task<Result> UpdatePetFavoriteActivitiesAsync(
      Guid clientId,
      Guid petId,
      string favoriteActivities,
      CancellationToken cancellationToken)
  {
    var client = await _repository.GetByIdAsync(clientId, cancellationToken);
    if (client == null)
    {
      return Result.NotFound("Client not found");
    }

    var pet = client.Pets.FirstOrDefault(p => p.Id == petId);
    if (pet == null)
    {
      return Result.NotFound("Pet not found");
    }

    pet.UpdateFavoriteActivities(favoriteActivities);
    await _repository.SaveChangesAsync(cancellationToken);

    return Result.Success();
  }

  public async Task<Result> UpdatePetDietaryRestrictionsAsync(
      Guid clientId,
      Guid petId,
      string dietaryRestrictions,
      CancellationToken cancellationToken)
  {
    var client = await _repository.GetByIdAsync(clientId, cancellationToken);
    if (client == null)
    {
      return Result.NotFound("Client not found");
    }

    var pet = client.Pets.FirstOrDefault(p => p.Id == petId);
    if (pet == null)
    {
      return Result.NotFound("Pet not found");
    }

    pet.UpdateDietaryRestrictions(dietaryRestrictions);
    await _repository.SaveChangesAsync(cancellationToken);

    return Result.Success();
  }

  public async Task<Result> UpdatePetSpecialNeedsAsync(
      Guid clientId,
      Guid petId,
      string specialNeeds,
      CancellationToken cancellationToken)
  {
    var client = await _repository.GetByIdAsync(clientId, cancellationToken);
    if (client == null)
    {
      return Result.NotFound("Client not found");
    }

    var pet = client.Pets.FirstOrDefault(p => p.Id == petId);
    if (pet == null)
    {
      return Result.NotFound("Pet not found");
    }

    pet.SpecialNeeds = specialNeeds;
    await _repository.SaveChangesAsync(cancellationToken);

    return Result.Success();
  }

  public async Task<Result> UpdatePetSterilizationStatusAsync(
      Guid clientId,
      Guid petId,
      bool isSterilized,
      CancellationToken cancellationToken)
  {
    var client = await _repository.GetByIdAsync(clientId, cancellationToken);
    if (client == null)
    {
      return Result.NotFound("Client not found");
    }

    var pet = client.Pets.FirstOrDefault(p => p.Id == petId);
    if (pet == null)
    {
      return Result.NotFound("Pet not found");
    }

    pet.IsSterilized = isSterilized;
    await _repository.SaveChangesAsync(cancellationToken);

    return Result.Success();
  }


}
