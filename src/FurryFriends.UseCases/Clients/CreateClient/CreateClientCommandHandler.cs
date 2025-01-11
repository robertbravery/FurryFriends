using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Services;

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

    var results = new bool[] { nameResult.IsSuccess, emailResult.IsSuccess, phoneResult.IsSuccess, addressResult.IsSuccess };
    if (results.All(x => x))
    {
      var result = await _clientService.CreateClientAsync(
          nameResult.Value,
          emailResult.Value,
          phoneResult.Value,
          addressResult.Value);

      return Result<Guid>.Success(result.Value.Id);
    }
    var errors = nameResult.Errors.Concat(emailResult.Errors).Concat(phoneResult.Errors).Concat(addressResult.Errors);
    return Result.Error(new ErrorList(errors));
  }
}
