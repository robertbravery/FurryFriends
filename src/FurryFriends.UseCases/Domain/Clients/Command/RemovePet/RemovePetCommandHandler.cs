using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.ClientAggregate.Specifications;
using Serilog;

namespace FurryFriends.UseCases.Domain.Clients.Command.RemovePet;

public class RemovePetCommandHandler(
    IRepository<Client> clientRepository,
    ILogger logger) : ICommandHandler<RemovePetCommand, Result>
{
  private readonly IRepository<Client> _clientRepository = clientRepository;
  private readonly ILogger _logger = logger;

  public async Task<Result> Handle(RemovePetCommand command, CancellationToken cancellationToken)
  {
    try
    {
      var spec = new ClientByIdWithPetsSpec(command.ClientId);
      var client = await _clientRepository.FirstOrDefaultAsync(spec, cancellationToken);
      if (client == null)
        return Result.NotFound("Client not found");

      var result = client.RemovePet(command.PetId);
      if (!result.IsSuccess)
        return result;

      await _clientRepository.UpdateAsync(client, cancellationToken);

      _logger.Information(
          "Pet {PetId} removed from client {ClientId}",
          command.PetId,
          command.ClientId);

      return Result.Success();
    }
    catch (Exception ex)
    {
      _logger.Error(ex,
          "Error removing pet {PetId} from client {ClientId}",
          command.PetId,
          command.ClientId);
      return Result.Error($"Error removing pet: {ex.Message}");
    }
  }
}
