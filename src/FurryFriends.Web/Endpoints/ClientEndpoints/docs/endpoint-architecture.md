# Client Endpoints Architecture

## Overview

The Client endpoints implement a RESTful API using FastEndpoints, following SOLID principles and clean architecture patterns.

```mermaid
graph TD
    subgraph API Layer
        E[Endpoints]
        V[Validators]
        M[Mappers]
        R[Requests/Responses]
    end
    
    subgraph Application Layer
        CMD[Commands]
        QRY[Queries]
        MED[Mediator]
    end
    
    subgraph Domain Layer
        AGG[Aggregates]
        VAL[Value Objects]
        DOM[Domain Events]
    end
    
    E --> V
    E --> M
    E --> R
    E --> MED
    MED --> CMD
    MED --> QRY
    CMD --> AGG
    QRY --> AGG
    AGG --> VAL
    AGG --> DOM
```

## Endpoint Implementation Pattern

```mermaid
classDiagram
    class BaseEndpoint {
        +Configure()
        +HandleAsync()
        #LogInformation()
        #LogError()
    }
    class CreateClient {
        -IMediator _mediator
        -ILogger _logger
        +Configure()
        +HandleAsync()
        -CreateCommand()
        -LogInformation()
        -LogError()
    }
    class ClientValidator {
        +ValidateAsync()
    }
    class ClientMapper {
        +ToEntity()
        +FromEntity()
    }
    
    BaseEndpoint <|-- CreateClient
    CreateClient --> ClientValidator
    CreateClient --> ClientMapper
```

## Endpoint Routes

| Method | Route | Description | Authentication |
|--------|-------|-------------|----------------|
| POST | /api/Clients | Create new client | Anonymous |
| PUT | /api/Clients/{id} | Update client | Anonymous |
| DELETE | /api/Clients/{id} | Delete client | Anonymous |
| GET | /api/Clients/{email} | Get client by email | Anonymous |
| GET | /api/Clients | List clients | Anonymous |

## Create Client Flow

```mermaid
sequenceDiagram
    participant C as Client
    participant E as Endpoint
    participant V as Validator
    participant M as Mediator
    participant H as Handler
    participant D as Domain
    
    C->>E: POST /api/Clients
    E->>V: Validate Request
    
    alt Invalid Request
        V-->>E: Validation Errors
        E-->>C: 400 Bad Request
    else Valid Request
        V-->>E: Success
        E->>M: Send Command
        M->>H: Handle Command
        H->>D: Create Client
        D-->>H: Client Created
        H-->>M: Success Result
        M-->>E: Client ID
        E-->>C: 200 OK
    end
```

## Update Client Flow

```mermaid
sequenceDiagram
    participant C as Client
    participant E as Endpoint
    participant V as Validator
    participant M as Mediator
    participant H as Handler
    
    C->>E: PUT /api/Clients/{id}
    E->>V: Validate Request
    
    alt Invalid Request
        V-->>E: Validation Errors
        E-->>C: 400 Bad Request
    else Valid Request
        V-->>E: Success
        E->>M: Send Command
        M->>H: Handle Command
        
        alt Client Not Found
            H-->>M: Not Found
            M-->>E: Not Found
            E-->>C: 404 Not Found
        else Success
            H-->>M: Success
            M-->>E: Updated Client
            E-->>C: 200 OK
        end
    end
```

## Validation Rules

```mermaid
graph TD
    A[Input Validation] --> B{Required Fields}
    B -->|Missing| C[Return 400]
    B -->|Present| D{Format Check}
    D -->|Invalid| E[Return 400]
    D -->|Valid| F{Business Rules}
    F -->|Failed| G[Return 400]
    F -->|Passed| H[Process Request]
```

### Request Validation Rules
```csharp
// Common validation rules for all client requests
RuleFor(x => x.FirstName).NotEmpty()
RuleFor(x => x.LastName).NotEmpty()
RuleFor(x => x.Email).NotEmpty().EmailAddress()
RuleFor(x => x.PhoneCountryCode).NotEmpty()
RuleFor(x => x.PhoneNumber).NotEmpty()
RuleFor(x => x.Street).NotEmpty()
RuleFor(x => x.City).NotEmpty()
RuleFor(x => x.State).NotEmpty()
RuleFor(x => x.Country).NotEmpty()
RuleFor(x => x.ZipCode).NotEmpty()
```

## Error Handling

```mermaid
graph TD
    A[Error Occurs] --> B{Error Type}
    B -->|Validation| C[Return 400]
    B -->|Not Found| D[Return 404]
    B -->|Business Rule| E[Return 400]
    B -->|Server Error| F[Return 500]
    
    C --> G[Log Error]
    D --> G
    E --> G
    F --> G
```

### Error Response Format
```json
{
  "type": "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1",
  "title": "Validation Error",
  "status": 400,
  "errors": [
    {
      "code": "ValidationError",
      "message": "The FirstName field is required"
    }
  ]
}
```

## Middleware Pipeline

```mermaid
graph LR
    A[Request] --> B[Exception Handler]
    B --> C[Authentication]
    C --> D[Validation]
    D --> E[Endpoint Handler]
    E --> F[Response]
```

## Request/Response Contracts

### Create Client
```csharp
public class CreateClientRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneCountryCode { get; set; }
    public string PhoneNumber { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string ZipCode { get; set; }
    public ClientType ClientType { get; set; }
    public TimeOnly? PreferredContactTime { get; set; }
    public ReferralSource? ReferralSource { get; set; }
}

public class CreateClientResponse
{
    public string ClientId { get; set; }
}
```

## Performance Considerations

1. **Caching Strategy**
   - Response caching for GET endpoints
   - Distributed caching for frequently accessed data
   - Cache invalidation on updates

2. **Query Optimization**
   - Pagination for list endpoints
   - Selective loading of related data
   - Optimized database queries

3. **Validation Performance**
   - Cached validator instances
   - Parallel validation where applicable
   - Early validation termination

## Security Considerations

1. **Input Validation**
   - Sanitize all inputs
   - Validate data types and ranges
   - Prevent injection attacks

2. **Authentication/Authorization**
   - JWT token validation
   - Role-based access control
   - API key validation

3. **Rate Limiting**
   - Per-client rate limits
   - Burst handling
   - DDoS protection