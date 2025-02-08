using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Services.ClientService;

namespace FurryFriends.UseCases.Clients.CreateClient;
internal class CreateClientCommandHandler : ICommandHandler<CreateClientCommand, Result<Guid>>
{
  private readonly IClientService _clientService;

  public CreateClientCommandHandler(IClientService clientService)
  {
    _clientService = clientService;
  }

  public async Task<Result<Guid>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
  {
    var nameResult = Name.Create(request.FirstName, request.LastName);
    var emailResult = Email.Create(request.Email);
    var phoneResult = await PhoneNumber.Create(request.CountryCode, request.PhoneNumber);
    var addressResult = Address.Create(request.Street, request.City, request.State, request.Country, request.ZipCode);

    var results = new IResult[]
    {
      nameResult,
      emailResult,
      phoneResult,
      addressResult
    };

    var errorsList = results.SelectMany(result => result.Errors);
    var validationErrorsList = results.SelectMany(result => result.ValidationErrors);

    if (errorsList.Any())
    {

      return Result.Error(new ErrorList(errorsList));
    }
    if (validationErrorsList.Any())
    {

      return Result.Error(new ErrorList(validationErrorsList.Select(x => x.ErrorMessage)));
    }

    var result = await _clientService.CreateClientAsync(
          nameResult.Value,
          emailResult.Value,
          phoneResult.Value,
          addressResult.Value, cancellationToken);

    return Result<Guid>.Success(result.Value.Id);
  }
}
