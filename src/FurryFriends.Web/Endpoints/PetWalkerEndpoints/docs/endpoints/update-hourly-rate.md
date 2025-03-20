# Update Hourly Rate Endpoint

## Flow Diagram

```mermaid
sequenceDiagram
    participant C as Client
    participant E as Endpoint
    participant V as Validator
    participant M as Mediator
    
    C->>E: PUT /api/pet-walkers/{id}/hourly-rate
    E->>V: Validate Request
    
    alt Invalid Request
        V-->>E: Validation Errors
        E-->>C: 400 Bad Request
    else Valid Request
        V-->>E: Success
        E->>M: UpdateHourlyRateCommand
        M-->>E: Success Result
        E-->>C: 200 OK
    end
```

## Request Contract
```json
{
  "hourlyRate": {
    "amount": "decimal",
    "currency": "string"
  }
}
```

## Validation Rules
```csharp
public class UpdateHourlyRateValidator : Validator<UpdateHourlyRateRequest>
{
    public UpdateHourlyRateValidator()
    {
        RuleFor(x => x.HourlyRate.Amount).GreaterThan(0);
        RuleFor(x => x.HourlyRate.Currency).NotEmpty();
    }
}
```