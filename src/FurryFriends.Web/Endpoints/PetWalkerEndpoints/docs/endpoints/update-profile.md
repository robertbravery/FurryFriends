# Update PetWalker Profile Endpoint

## Flow Diagram

```mermaid
sequenceDiagram
    participant C as Client
    participant E as Endpoint
    participant V as Validator
    participant M as Mediator
    participant H as Handler
    participant D as Domain
    
    C->>E: PUT /api/pet-walkers/{id}/profile
    E->>V: Validate Request
    
    alt Invalid Request
        V-->>E: Validation Errors
        E-->>C: 400 Bad Request
    else Valid Request
        V-->>E: Success
        E->>M: UpdateProfileCommand
        M->>H: Handle Command
        H->>D: Update Profile
        D-->>H: Profile Updated
        H-->>M: Success Result
        M-->>E: Success
        E-->>C: 200 OK
    end
```

## Request Contract
```json
{
  "biography": "string",
  "maxPetsPerWalk": "number",
  "yearsOfExperience": "number",
  "hasInsurance": "boolean",
  "hasFirstAidCertification": "boolean"
}
```

## Validation Rules
```csharp
public class UpdateProfileValidator : Validator<UpdateProfileRequest>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.Biography).MaximumLength(1000);
        RuleFor(x => x.MaxPetsPerWalk).InclusiveBetween(1, 5);
        RuleFor(x => x.YearsOfExperience).GreaterThanOrEqualTo(0);
    }
}
```