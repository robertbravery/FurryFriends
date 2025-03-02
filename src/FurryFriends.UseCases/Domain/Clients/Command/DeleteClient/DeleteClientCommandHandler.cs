using FurryFriends.Core.ClientAggregate;
using MediatR;

namespace FurryFriends.UseCases.Domain.Clients.Command.DeleteClient;

public class DeleteClientCommandHandler(IRepository<Client> _clientRepository)
    : IRequestHandler<DeleteClientCommand, Result>
{
  public async Task<Result> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
  {
    var client = await _clientRepository.GetByIdAsync(request.ClientId);

    if (client is null)
      return Result.NotFound();

    await _clientRepository.DeleteAsync(client);
    return Result.Success();
  }
}
