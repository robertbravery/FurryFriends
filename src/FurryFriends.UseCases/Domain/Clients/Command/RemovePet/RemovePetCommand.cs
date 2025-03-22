using MediatR;

namespace FurryFriends.UseCases.Domain.Clients.Command.RemovePet;

public record RemovePetCommand(Guid ClientId, Guid PetId) : ICommand<Result>;