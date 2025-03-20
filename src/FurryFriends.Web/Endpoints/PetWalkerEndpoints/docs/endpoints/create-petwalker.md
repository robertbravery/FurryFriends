# Create PetWalker Endpoint

## Flow Diagram

```mermaid
sequenceDiagram
    participant C as Client
    participant E as Endpoint
    participant V as Validator
    participant M as Mediator
    participant H as Handler
    participant D as Domain
    
    C->>E: POST /api/pet-walkers
    E->>V: Validate Request
    
    alt Invalid Request
        V-->>E: Validation Errors
        E-->>C: 400 Bad Request
    else Valid Request
        V-->>E: Success
        E->>M: CreatePetWalkerCommand
        M->>H: Handle Command
        H->>D: Create PetWalker
        D-->>H: PetWalker Created
        H-->>M: Success Result
        M-->>E: PetWalker ID
        E-->>C: 200 OK
    end
```

## Request Contract
```json
{
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "phoneNumber": {
    "countryCode": "string",
    "number": "string"
  },
  "address": {
    "street": "string",
    "city": "string",
    "state": "string",
    "country": "string",
    "zipCode": "string"
  },
  "dateOfBirth": "string (yyyy-MM-dd)",
  "gender": "string",
  "biography": "string",
  "hourlyRate": {
    "amount": "decimal",
    "currency": "string"
  }
}
```

## Validation Rules
```csharp
public class CreatePetWalkerValidator : Validator<CreatePetWalkerRequest>
{
    public CreatePetWalkerValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.DateOfBirth).Must(BeAtLeast18YearsOld);
        RuleFor(x => x.Biography).MaximumLength(1000);
        RuleFor(x => x.HourlyRate.Amount).GreaterThan(0);
    }
}
```