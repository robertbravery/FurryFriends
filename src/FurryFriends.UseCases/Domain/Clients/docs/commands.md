# Client Commands

## Available Commands

### 1. CreateClientCommand
Creates a new client in the system.
```csharp
public record CreateClientCommand(
    string FirstName,
    string LastName,
    string Email,
    string CountryCode,
    string PhoneNumber,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    ClientType ClientType = ClientType.Regular,
    TimeOnly? PreferredContactTime = null,
    ReferralSource ReferralSource = ReferralSource.None
) : ICommand<Result<Guid>>;
```

### 2. UpdateClientCommand
Updates existing client information.
```csharp
public record UpdateClientCommand(
    Guid ClientId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    Address Address,
    ClientType? ClientType,
    TimeOnly? PreferredContactTime
) : ICommand<Result>;
```

### 3. AddPetCommand
Adds a new pet to an existing client.
```csharp
public record AddPetCommand(
    Guid ClientId,
    string PetName,
    string Species,
    int BreedId,
    double Weight,
    string Color
) : ICommand<Result<Guid>>;
```

## Command Validation
Each command has its own validator:
```csharp
public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        // Additional rules...
    }
}
```