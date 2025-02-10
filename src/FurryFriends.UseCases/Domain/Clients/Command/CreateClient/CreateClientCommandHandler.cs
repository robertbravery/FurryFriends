using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Services.ClientService;

namespace FurryFriends.UseCases.Domain.Clients.Command.CreateClient;
internal class CreateClientCommandHandler : ICommandHandler<CreateClientCommand, Result<Guid>>
{
  private readonly IClientService _clientService;

  public CreateClientCommandHandler(IClientService clientService)
  {
    _clientService = clientService;
  }

  public async Task<Result<Guid>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
  {

    var creationResults = new IResult[]
    {
      Name.Create(request.FirstName, request.LastName),
      Email.Create(request.Email),
      await PhoneNumber.Create(request.CountryCode, request.PhoneNumber),
      Address.Create(request.Street, request.City, request.State, request.Country, request.ZipCode)
    };

    var errors = creationResults.SelectMany(r => r.Errors);
    var validationErrors = creationResults.SelectMany(r => r.ValidationErrors);
    if (errors.Any() || validationErrors.Any())
    {
      return Result.Error(new ErrorList(errors.Concat(validationErrors.Select(e => e.ErrorMessage))));
    }

    var clientCreationResult = await _clientService.CreateClientAsync(
    (Name)creationResults[0].GetValue(),
    (Email)creationResults[1].GetValue(),
    (PhoneNumber)creationResults[2].GetValue(),
    (Address)creationResults[3].GetValue(),
    cancellationToken);
    return clientCreationResult.IsSuccess
      ? Result<Guid>.Success(clientCreationResult.Value.Id)
      : Result.Error(new ErrorList(clientCreationResult.Errors));
  }
}
