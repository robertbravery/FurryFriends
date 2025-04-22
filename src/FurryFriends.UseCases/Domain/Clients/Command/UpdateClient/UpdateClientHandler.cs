using Ardalis.GuardClauses;
using FluentValidation;
using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Services.ClientService;
using MediatR;

namespace FurryFriends.UseCases.Domain.Clients.Command.UpdateClient;

public class UpdateClientCommandHandler : ICommandHandler<UpdateClientCommand, Result<Client>>
{
  private readonly IReadRepository<Client> _clientRepository;
  private readonly IClientService _clientService;

  public UpdateClientCommandHandler(IReadRepository<Client> _clientRepository, IClientService clientService)
  {
    this._clientRepository = _clientRepository;
    _clientService = clientService;
  }

  public async Task<Result<Client>> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
  {
    Guard.Against.Null(request, nameof(request));
    Guard.Against.NullOrEmpty(request.ClientId, nameof(request.ClientId));
    
    Client client = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken) 
        ?? throw new NotFoundException(nameof(Client), request.ClientId.ToString());

    var nameResult = Name.Create(request.FirstName, request.LastName);
    var emailResult = Email.Create(request.Email);
    var phoneResult = await PhoneNumber.Create(request.CountryCode, request.PhoneNumber);
    var addressResult = Address.Create(request.Street, request.City, request.StateProvinceRegion, request.Country, request.ZipCode);


    if (!nameResult.IsSuccess || !emailResult.IsSuccess || !phoneResult.IsSuccess || !addressResult.IsSuccess)
    {
        var errors = new List<string>();
        errors.AddRange(nameResult.Errors);
        errors.AddRange(emailResult.Errors);
        errors.AddRange(phoneResult.Errors);
        errors.AddRange(addressResult.Errors);

        errors.AddRange(nameResult.ValidationErrors?.Select(v => v.ErrorMessage) ?? []);
        errors.AddRange(emailResult.ValidationErrors?.Select(v => v.ErrorMessage) ?? []);
        errors.AddRange(phoneResult.ValidationErrors?.Select(v => v.ErrorMessage) ?? []);
        errors.AddRange(addressResult.ValidationErrors?.Select(v => v.ErrorMessage) ?? []);

        throw new ValidationException(string.Join(", ", errors));
    }

    client.UpdateDetails(
        nameResult.Value,
        emailResult.Value,
        phoneResult.Value,
        addressResult.Value
    );

    client.UpdateClientType(request.ClientType);
    client.UpdatePreferredContactTime(request.PreferredContactTime);
    client.UpdateReferralSource(request.ReferralSource);

    var updateClient = await _clientService.UpdateClientAsync(client);
return updateClient;
    // return updateClient == null ? Result.Success(updateClient) : Result.NotFound();
  }
}