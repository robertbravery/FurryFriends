# Error Handling Strategies

## Overview
The application layer implements a comprehensive error handling strategy using the Result pattern and domain-specific exceptions.

## Result Pattern
```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public IReadOnlyList<string> Errors { get; }
    public IReadOnlyList<ValidationError> ValidationErrors { get; }
    public ResultStatus Status { get; }
}
```

## Error Categories

### 1. Validation Errors
- Pre-execution validation using FluentValidation
- Domain validation during entity/value object creation
- Business rule validation

```csharp
public class CreateClientHandler
{
    public async Task<Result<Guid>> Handle(CreateClientCommand command)
    {
        // FluentValidation
        var validationResult = await _validator.ValidateAsync(command);
        if (!validationResult.IsValid)
            return Result.Invalid(validationResult.Errors);

        // Domain Validation
        var nameResult = Name.Create(command.FirstName, command.LastName);
        if (!nameResult.IsSuccess)
            return Result.Invalid(nameResult.ValidationErrors);

        // Business Rule Validation
        if (await _clientService.HasReachedMaxClientsLimit())
            return Result.Error("Maximum client limit reached");
    }
}
```

### 2. Domain Errors
- Handled through domain-specific exceptions
- Mapped to appropriate Result types

```csharp
try
{
    await _clientService.UpdateClientType(command.ClientId, command.NewType);
}
catch (ClientDomainException ex)
{
    return Result.Error(ex.Message);
}
```

### 3. Infrastructure Errors
- Database connectivity issues
- External service failures
- Mapped to appropriate error codes

```csharp
try
{
    await _repository.SaveChangesAsync();
}
catch (DbUpdateConcurrencyException)
{
    return Result.Error("CL006", "Concurrent modification detected");
}
```

## Error Codes and Messages

### Client Error Codes
| Code  | Description               | HTTP Status |
|-------|---------------------------|-------------|
| CL001 | Invalid client data      | 400         |
| CL002 | Email already exists     | 409         |
| CL003 | Client not found         | 404         |
| CL004 | Pet limit exceeded       | 422         |
| CL005 | Invalid pet data         | 400         |
| CL006 | Concurrent modification  | 409         |

## Logging Strategy
```csharp
public class UpdateClientHandler
{
    public async Task<Result> Handle(UpdateClientCommand command)
    {
        try
        {
            _logger.LogInformation("Updating client {ClientId}", command.ClientId);
            // ... handling logic
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update client {ClientId}", command.ClientId);
            return Result.Error("Unexpected error updating client");
        }
    }
}
```