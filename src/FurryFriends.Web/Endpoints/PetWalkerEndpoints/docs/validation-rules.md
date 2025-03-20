# PetWalker Endpoint Validation Rules

## Common Rules

### Personal Information
```csharp
RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
RuleFor(x => x.Email).NotEmpty().EmailAddress();
RuleFor(x => x.PhoneNumber.CountryCode).NotEmpty();
RuleFor(x => x.PhoneNumber.Number).NotEmpty();
```

### Address
```csharp
RuleFor(x => x.Address.Street).NotEmpty();
RuleFor(x => x.Address.City).NotEmpty();
RuleFor(x => x.Address.State).NotEmpty();
RuleFor(x => x.Address.ZipCode).NotEmpty();
```

### Business Rules
```csharp
RuleFor(x => x.DateOfBirth).Must(BeAtLeast18YearsOld);
RuleFor(x => x.Biography).MaximumLength(1000);
RuleFor(x => x.MaxPetsPerWalk).InclusiveBetween(1, 5);
RuleFor(x => x.HourlyRate.Amount).GreaterThan(0);
```

## Error Responses

### Validation Error
```json
{
  "type": "ValidationError",
  "title": "One or more validation errors occurred",
  "status": 400,
  "errors": {
    "FirstName": ["First name is required"],
    "Email": ["Invalid email format"]
  }
}
```

### Business Rule Error
```json
{
  "type": "BusinessRuleViolation",
  "title": "Business rule violation",
  "status": 400,
  "detail": "Must be at least 18 years old"
}
```