# Specific Use Case Implementations

## 1. Client Registration
Handles new client registration with validation and business rules.

### Command Flow
```csharp
public class CreateClientHandler
{
    public async Task<Result<Guid>> Handle(CreateClientCommand command)
    {
        // 1. Validate input
        var validationResult = await _validator.ValidateAsync(command);
        if (!validationResult.IsValid)
            return Result.Invalid(validationResult.Errors);

        // 2. Check business rules
        if (await _clientService.EmailExists(command.Email))
            return Result.Error("CL002", "Email already exists");

        // 3. Create domain objects
        var client = await _clientService.CreateClientAsync(command);

        // 4. Persist changes
        await _repository.AddAsync(client);
        await _repository.SaveChangesAsync();

        // 5. Raise domain events
        await _mediator.Publish(new ClientCreatedEvent(client.Id));

        return Result.Success(client.Id);
    }
}
```

## 2. Client Profile Update
Handles updating existing client information with concurrency checks.

### Implementation
```csharp
public class UpdateClientHandler
{
    public async Task<Result> Handle(UpdateClientCommand command)
    {
        // 1. Retrieve existing client
        var client = await _clientService.GetClientAsync(command.ClientId);
        if (client == null)
            return Result.NotFound();

        // 2. Apply updates
        client.UpdateProfile(
            command.FirstName,
            command.LastName,
            command.PhoneNumber,
            command.Address);

        // 3. Handle concurrency
        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Error("CL006", "Profile was updated by another user");
        }

        return Result.Success();
    }
}
```

## 3. Add Pet to Client
Handles adding a new pet to an existing client with validation.

### Implementation
```csharp
public class AddPetHandler
{
    public async Task<Result<Guid>> Handle(AddPetCommand command)
    {
        // 1. Validate client exists
        var client = await _clientService.GetClientAsync(command.ClientId);
        if (client == null)
            return Result.NotFound("Client not found");

        // 2. Check pet limit
        if (client.HasReachedPetLimit())
            return Result.Error("CL004", "Maximum pets limit reached");

        // 3. Create and add pet
        var pet = Pet.Create(command.PetName, command.Species);
        client.AddPet(pet);

        // 4. Save changes
        await _repository.SaveChangesAsync();

        return Result.Success(pet.Id);
    }
}
```

## 4. Client Search and Filtering
Implements advanced search functionality for clients.

### Query Implementation
```csharp
public class SearchClientsHandler
{
    public async Task<Result<PagedList<ClientDTO>>> Handle(
        SearchClientsQuery query,
        CancellationToken cancellationToken)
    {
        var specification = new ClientSearchSpecification(
            query.NameSearch,
            query.EmailSearch,
            query.ClientType);

        var clients = await _repository.ListAsync(
            specification
                .Skip(query.Page * query.PageSize)
                .Take(query.PageSize));

        var totalCount = await _repository.CountAsync(specification);

        return Result.Success(new PagedList<ClientDTO>(
            clients.Select(_mapper.Map<ClientDTO>).ToList(),
            totalCount,
            query.Page,
            query.PageSize));
    }
}
```

## 5. Client Type Upgrade
Handles client type upgrades with business rules.

### Implementation
```csharp
public class UpgradeClientHandler
{
    public async Task<Result> Handle(UpgradeClientCommand command)
    {
        // 1. Get client
        var client = await _clientService.GetClientAsync(command.ClientId);
        if (client == null)
            return Result.NotFound();

        // 2. Check eligibility
        if (!client.IsEligibleForUpgrade(command.NewType))
            return Result.Error("Client not eligible for upgrade");

        // 3. Apply upgrade
        client.UpgradeType(command.NewType);
        
        // 4. Save changes
        await _repository.SaveChangesAsync();

        // 5. Notify about upgrade
        await _mediator.Publish(new ClientUpgradedEvent(client.Id, command.NewType));

        return Result.Success();
    }
}
```