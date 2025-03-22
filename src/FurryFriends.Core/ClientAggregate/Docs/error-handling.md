# Error Handling Guidelines

## Exception Types

### Domain Exceptions
```csharp
public class ClientDomainException : DomainException
{
    public ClientDomainException(string message) : base(message) { }
}

public class PetDomainException : DomainException
{
    public PetDomainException(string message) : base(message) { }
}
```

### Error Codes
- CL001: Invalid client data
- CL002: Email already exists
- CL003: Client not found
- CL004: Pet limit exceeded
- CL005: Invalid pet data
- CL006: Concurrent modification
- CL007: Invalid operation for client type
- CL008: Address validation failed

## HTTP Status Codes
- 400: Bad Request (validation errors)
- 401: Unauthorized
- 403: Forbidden (operation not allowed)
- 404: Not Found
- 409: Conflict (concurrent modification)
- 422: Unprocessable Entity (business rule violations)

## Error Response Format
```json
{
  "type": "https://furry-friends.com/errors/cl001",
  "title": "Validation Error",
  "status": 400,
  "detail": "The provided client data is invalid",
  "instance": "/api/clients/create",
  "errors": [
    {
      "code": "CL001",
      "field": "email",
      "message": "Email format is invalid"
    }
  ]
}
```