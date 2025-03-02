using MediatR;

namespace FurryFriends.UseCases.Domain.Clients.Command.DeleteClient;

public record DeleteClientCommand(Guid ClientId) : IRequest<Result>;
