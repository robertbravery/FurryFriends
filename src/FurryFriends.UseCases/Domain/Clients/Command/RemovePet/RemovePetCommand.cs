using MediatR;

namespace FurryFriends.UseCases.Domain.Clients.Command.RemovePet;

public record RemovePetCommand(Guid PetId) : ICommand<Result>;
