﻿using FurryFriends.Core.ClientAggregate;
using FurryFriends.UseCases.Domain.Clients.Command.UpdatePetInfo;

namespace FurryFriends.UseCases.Domain.Clients.Command.UpdatePetInfo;

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
            command.MedicalHistory,
            command.IsVaccinated,
            command.FavoriteActivities,
            command.DietaryRestrictions,
            command.SpecialNeeds,
            command.Photo,
            command.BreedId);

        if (!result.IsSuccess)
            return result;

        await _clientRepository.UpdateAsync(client, cancellationToken);
        return Result.Success();
    }
}
