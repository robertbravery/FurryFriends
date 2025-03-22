# Command and Query Handlers

## Command Handlers

### CreateClientHandler
```csharp
public class CreateClientHandler : ICommandHandler<CreateClientCommand, Result<Guid>>
{
    private readonly IClientService _clientService;
    private readonly IValidator<CreateClientCommand> _validator;

    public async Task<Result<Guid>> Handle(CreateClientCommand command, CancellationToken cancellationToken)
    {
        // Validation
        var validationResult = await _validator.ValidateAsync(command);
        if (!validationResult.IsValid)
            return Result.ValidationError(validationResult.Errors);

        // Create domain objects
        var nameResult = Name.Create(command.FirstName, command.LastName);
        var emailResult = Email.Create(command.Email);
        // ... additional value object creation

        // Create and save client
        var client = await _clientService.CreateClientAsync(/* parameters */);
        return Result.Success(client.Id);
    }
}
```

## Query Handlers

### GetClientQueryHandler
```csharp
public class GetClientQueryHandler : IQueryHandler<GetClientQuery, Result<ClientDTO>>
{
    private readonly IClientService _clientService;
    private readonly IMapper _mapper;

    public async Task<Result<ClientDTO>> Handle(GetClientQuery query, CancellationToken cancellationToken)
    {
        var client = await _clientService.GetClientAsync(query.EmailAddress);
        if (client == null)
            return Result.NotFound();

        var dto = _mapper.Map<ClientDTO>(client);
        return Result.Success(dto);
    }
}
```