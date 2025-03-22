using FurryFriends.Core.ClientAggregate;

public class UpdatePetInfoHandler : ICommandHandler<UpdatePetInfoCommand, Result>
{
    private readonly IRepository<Client> _clientRepository;

    public UpdatePetInfoHandler(IRepository<Client> clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<Result> Handle(UpdatePetInfoCommand command, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetByIdAsync(command.ClientId, cancellationToken);
        if (client == null)
            return Result.Error("Client not found");

        var result = client.UpdatePetInfo(
            command.PetId,
            command.Name,
            command.Age,
            command.Weight,
            command.Color,
            command.DietaryRestrictions,
            command.FavoriteActivities);

        if (!result.IsSuccess)
            return result;

        await _clientRepository.UpdateAsync(client, cancellationToken);
        return Result.Success();
    }
}