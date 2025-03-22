# PetWalker Use Case Specifications

## Command Use Cases

### CreatePetWalker
- **Actor**: Prospective Pet Walker
- **Preconditions**: None
- **Success Scenario**:
  1. User submits registration information
  2. System validates input
  3. System creates PetWalker profile
  4. System initiates verification process
  5. System returns success with ID
- **Alternative Flows**:
  - Invalid input: Return validation errors
  - Duplicate email: Return conflict error
  - System error: Return error status

### UpdatePetWalker
- **Actor**: Registered Pet Walker
- **Preconditions**: Active profile exists
- **Success Scenario**:
  1. User submits profile updates
  2. System validates changes
  3. System applies updates
  4. System publishes update event
  5. System returns success
- **Alternative Flows**:
  - Invalid changes: Return validation errors
  - Profile locked: Return error status
  - Concurrent update: Handle conflict

### DeletePetWalker
- **Actor**: Registered Pet Walker or Admin
- **Preconditions**: Active profile exists
- **Success Scenario**:
  1. User requests deletion
  2. System checks for active bookings
  3. System deactivates profile
  4. System publishes deletion event
  5. System returns success
- **Alternative Flows**:
  - Active bookings: Return business error
  - Profile locked: Return error status

## Query Use Cases

### GetPetWalker
- **Actor**: Any authenticated user
- **Preconditions**: None
- **Success Scenario**:
  1. User requests profile details
  2. System retrieves profile
  3. System returns profile DTO
- **Alternative Flows**:
  - Not found: Return not found status
  - System error: Return error status

### ListPetWalkers
- **Actor**: Any authenticated user
- **Preconditions**: None
- **Success Scenario**:
  1. User submits search criteria
  2. System applies filters
  3. System returns paged results
- **Alternative Flows**:
  - Invalid criteria: Return validation errors
  - No results: Return empty list

## Data Contracts

### CreatePetWalkerCommand
```csharp
public record CreatePetWalkerCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    Address Address,
    DateTime DateOfBirth,
    string Biography,
    decimal HourlyRate,
    bool HasInsurance,
    bool HasFirstAidCert
);
```

### UpdatePetWalkerCommand
```csharp
public record UpdatePetWalkerCommand(
    Guid Id,
    string? Biography,
    decimal? HourlyRate,
    Address? Address,
    string? PhoneNumber,
    bool? IsActive
);
```

### PetWalkerDTO
```csharp
public record PetWalkerDTO(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    Address Address,
    string Biography,
    decimal HourlyRate,
    double Rating,
    bool IsActive,
    bool IsVerified
);
```

## Integration Events

### PetWalkerCreated
```csharp
public record PetWalkerCreated(
    Guid Id,
    string Email,
    DateTime CreatedAt
);
```

### PetWalkerVerified
```csharp
public record PetWalkerVerified(
    Guid Id,
    DateTime VerifiedAt
);
```

### PetWalkerDeactivated
```csharp
public record PetWalkerDeactivated(
    Guid Id,
    DateTime DeactivatedAt,
    string Reason
);
```