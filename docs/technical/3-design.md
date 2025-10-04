# Design Documentation

## 1. Domain Model

### 1.1 Core Aggregates

```mermaid
classDiagram
    class Client {
        +Id: Guid
        +Name: Name
        +Email: Email
        +PhoneNumber: PhoneNumber
        +Address: Address
        +ClientType: ClientType
        +ReferralSource: ReferralSource
        +IsActive: bool
        +Pets: List~Pet~
        +CreateClient()
        +AddPet()
        +UpdateDetails()
    }

    class Pet {
        +Id: Guid
        +Name: string
        +BreedId: int
        +Age: int
        +Weight: double
        +Color: string
        +SpecialNeeds: string
        +DietaryRestrictions: string
        +IsSterilized: bool
        +IsVaccinated: bool
        +UpdateDetails()
    }

    class PetWalker {
        +Id: Guid
        +Name: Name
        +Email: Email
        +PhoneNumber: PhoneNumber
        +ServiceArea: ServiceArea
        +Availability: List~TimeSlot~
        +Certifications: List~Certification~
        +HourlyRate: Money
        +CreatePetWalker()
        +UpdateAvailability()
    }

    Client "1" --o "*" Pet : Owns
    PetWalker "1" --o "*" ServiceArea : Serves

```

### 1.2 Value Objects

```mermaid
classDiagram
    class Name {
        +FirstName: string
        +LastName: string
        +Create()
    }

    class Email {
        +EmailAddress: string
        +Create()
        +Validate()
    }

    class PhoneNumber {
        +CountryCode: string
        +Number: string
        +Create()
        +Format()
    }

    class Address {
        +Street: string
        +City: string
        +StateProvinceRegion: string
        +Country: string
        +ZipCode: string
        +Create()
    }

    class Money {
        +Amount: decimal
        +Currency: string
        +Create()
        +Add()
        +Subtract()
    }
```

## 2. Design Patterns

### 2.1 Clean Architecture
The solution follows Clean Architecture principles with clear separation of concerns:

```mermaid
graph TB
    A[Web API / Blazor UI] --> B[Use Cases]
    B --> C[Domain Model]
    D[Infrastructure] --> B
    D --> C
```

Layers:
- **Domain Layer**: Core business logic and entities
- **Use Cases Layer**: Application services and business operations
- **Infrastructure Layer**: External concerns (data access, messaging)
- **Presentation Layer**: UI and API endpoints

### 2.2 Domain-Driven Design (DDD)
- **Aggregates**: Client, PetWalker as aggregate roots
- **Value Objects**: Name, Email, PhoneNumber, Address
- **Domain Services**: Booking coordination, payment processing
- **Specifications**: For encapsulating query logic

### 2.3 CQRS Implementation
Commands and Queries are separated:

#### Commands:
- CreateClient
- UpdateClientDetails
- AddPetToClient
- UpdatePetDetails

#### Queries:
- GetClientByEmail
- ListClientsByLocation
- GetPetWalkerAvailability
- ListBookings

## 3. Database Schema

```mermaid
erDiagram
    Clients ||--o{ Pets : "owns"
    Clients {
        Guid Id
        string FirstName
        string LastName
        string Email
        string PhoneNumber
        string Address
        int ClientType
        datetime CreatedAt
    }
    Pets {
        Guid Id
        Guid OwnerId
        string Name
        int BreedId
        int Age
        float Weight
        string Color
        string SpecialNeeds
        boolean IsVaccinated
    }
    PetWalkers ||--o{ ServiceAreas : "serves"
    PetWalkers {
        Guid Id
        string FirstName
        string LastName
        string Email
        string PhoneNumber
        decimal HourlyRate
        string Certifications
    }
    ServiceAreas {
        Guid Id
        Guid PetWalkerId
        string Region
        float Radius
    }
    Bookings ||--|| Clients : "books"
    Bookings ||--|| PetWalkers : "serves"
    Bookings {
        Guid Id
        Guid ClientId
        Guid PetWalkerId
        datetime StartTime
        datetime EndTime
        decimal TotalAmount
        int Status
    }
```

## 4. API Contracts

### 4.1 REST Endpoints

#### Client Management
```yaml
/api/clients:
  post:
    summary: Create new client
    request:
      body:
        name: { firstName, lastName }
        email: string
        phoneNumber: { countryCode, number }
        address: { street, city, state, country, zip }
    response:
      201: { clientId: guid }
      400: { errors: [] }

/api/clients/{email}:
  get:
    summary: Get client by email
    response:
      200: { clientDetails }
      404: { error: string }

/api/clients/{id}/pets:
  post:
    summary: Add pet to client
    request:
      body:
        name: string
        breedId: int
        age: int
        weight: float
    response:
      201: { petId: guid }
      400: { errors: [] }
```

### 4.2 Authentication Flow

```mermaid
sequenceDiagram
    participant Client
    participant API
    participant Auth
    participant Database

    Client->>API: POST /auth/login
    API->>Auth: Validate Credentials
    Auth->>Database: Verify User
    Database-->>Auth: User Details
    Auth-->>API: Generate JWT
    API-->>Client: Return JWT Token
```

## 5. Security Model

### 5.1 Authentication
- JWT-based authentication
- Token expiration and refresh mechanism
- Role-based access control (Client, PetWalker, Admin)

### 5.2 Authorization Policies
```csharp
services.AddAuthorization(options =>
{
    options.AddPolicy("ClientPolicy", policy =>
        policy.RequireRole("Client"));
    
    options.AddPolicy("PetWalkerPolicy", policy =>
        policy.RequireRole("PetWalker"));
        
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
});
```

### 5.3 Data Protection
- Encryption at rest for sensitive data
- TLS 1.3 for data in transit
- Password hashing using BCrypt
- Input validation and sanitization