using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.ClientAggregate.Specifications;
using FurryFriends.Core.Interfaces;
using Serilog;

namespace FurryFriends.UseCases.Domain.Clients.Command.RemovePet;

public class RemovePetCommandHandler(
    IRepository<Client> clientRepository,
    IRepository<Pet> petRepository,
    ILogger logger) : ICommandHandler<RemovePetCommand, Result>
{
  private readonly IRepository<Client> _clientRepository = clientRepository;
  private readonly IRepository<Pet> _petRepository = petRepository;
  private readonly ILogger _logger = logger;

  public async Task<Result> Handle(RemovePetCommand command, CancellationToken cancellationToken)
  {
    try
    {
      var pet = await _petRepository.GetByIdAsync(command.PetId, cancellationToken);

      if (pet == null)
        return Result.NotFound("Pet not found");

      var client = await _clientRepository.GetByIdAsync(pet.OwnerId, cancellationToken);
      if (client == null)
        return Result.NotFound("Client not found");

      var result = client.RemovePet(command.PetId);
      if (!result.IsSuccess)
        return result;

      await _clientRepository.UpdateAsync(client, cancellationToken);

      _logger.Information(
          "Pet {PetId} removed from client {ClientId}",
          command.PetId,
          client.Id);

      return Result.Success();
    }
    catch (Exception ex)
    {
      _logger.Error(ex,
          "Error removing pet {PetId} from client",
          command.PetId);
      return Result.Error($"Error removing pet: {ex.Message}");
    }
  }
}
