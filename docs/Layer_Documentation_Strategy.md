# Layer-Specific Documentation Strategy

## Core Layer (`src/FurryFriends.Core/`)
Documentation focus: Domain model, business rules, and invariants

```
src/FurryFriends.Core/
├── ClientAggregate/
│   ├── docs/
│   │   ├── domain-model.md           # Entity relationships, invariants
│   │   ├── value-objects.md          # Value objects documentation
│   │   └── diagrams/
│   │       ├── client-aggregate.mmd  # Aggregate structure
│   │       └── pet-relationships.mmd # Pet-related relationships
├── PetWalkerAggregate/
│   ├── docs/
│   │   ├── domain-model.md
│   │   ├── service-areas.md          # Service area rules
│   │   └── diagrams/
│   │       ├── walker-aggregate.mmd
│   │       └── scheduling-rules.mmd
└── LocationAggregate/
    └── docs/
        ├── location-hierarchy.md      # Country/Region/Locality rules
        └── diagrams/
            └── location-model.mmd
```

## Use Cases Layer (`src/FurryFriends.UseCases/`)
Documentation focus: Application services, CQRS, business workflows

```
src/FurryFriends.UseCases/
├── PetWalking/
│   ├── docs/
│   │   ├── booking-flow.md           # Booking process
│   │   ├── scheduling-rules.md       # Scheduling logic
│   │   └── diagrams/
│   │       ├── booking-sequence.mmd  # Sequence diagram
│   │       └── state-machine.mmd     # Booking states
├── UserManagement/
│   └── docs/
│       ├── registration-flow.md      # Registration process
│       └── diagrams/
│           └── verification-flow.mmd # User verification
└── Payments/
    └── docs/
        ├── payment-flow.md           # Payment processing
        └── diagrams/
            └── payment-sequence.mmd
```

## Infrastructure Layer (`src/FurryFriends.Infrastructure/`)
Documentation focus: Technical implementation, external services, persistence

```
src/FurryFriends.Infrastructure/
├── Data/
│   ├── docs/
│   │   ├── database-schema.md        # Database structure
│   │   ├── migrations-guide.md       # Migration strategy
│   │   └── diagrams/
│   │       ├── erd.mmd              # Entity-Relationship Diagram
│   │       └── indexes.md           # Database indexes
├── Services/
│   └── docs/
│       ├── external-services.md      # Third-party integrations
│       └── diagrams/
│           └── service-integration.mmd
└── Security/
    └── docs/
        ├── authentication.md         # Auth implementation
        └── authorization.md          # Authorization rules
```

## Web API Layer (`src/FurryFriends.Web/`)
Documentation focus: API endpoints, controllers, DTOs

```
src/FurryFriends.Web/
├── Endpoints/
│   └── docs/
│       ├── api-conventions.md        # API standards
│       ├── error-handling.md         # Error response format
│       └── endpoints/
│           ├── booking-api.md        # Booking endpoints
│           └── user-api.md           # User endpoints
├── DTOs/
│   └── docs/
│       └── dto-mappings.md          # DTO to domain mappings
└── docs/
    ├── api-documentation.md         # OpenAPI/Swagger docs
    └── diagrams/
        └── api-flow.mmd            # API request flow
```

## Central Documentation (`/docs/`)
Cross-cutting and architectural documentation

```
docs/
├── architecture/
│   ├── decisions/                   # ADRs
│   │   ├── adr-001-clean-arch.md
│   │   └── adr-002-cqrs.md
│   ├── c4-models/
│   │   ├── context.mmd             # Your existing C4 diagrams
│   │   └── containers.mmd
│   └── patterns/
│       └── implemented-patterns.md
├── requirements/
│   ├── user-stories/
│   └── technical-specifications/
└── development/
    ├── setup.md
    ├── conventions.md
    └── best-practices.md
```

## Key Documentation Types by Layer

### Core Layer
- Domain model diagrams
- Aggregate boundaries
- Business rules and invariants
- Value object specifications
- Domain events documentation

### Use Cases Layer
- CQRS command/query documentation
- Business process flows
- Sequence diagrams
- State machines
- Validation rules

### Infrastructure Layer
- Database schemas
- Migration guides
- External service integration
- Configuration details
- Security implementation

### Web API Layer
- API documentation
- Endpoint specifications
- DTO mappings
- Authentication flows
- Error handling

## Implementation Notes

1. **Existing Documentation Integration**
   - Your existing mermaid diagrams (Architecture 1.mmd, Architecture 2.mmd) should move to `/docs/architecture/`
   - User Aggregate diagrams should be split between Core layer documentation and central architecture docs
   - C4 Model diagrams should remain in central docs as they're cross-cutting

2. **Documentation Format**
   - Continue using Mermaid for diagrams
   - Maintain markdown for written documentation
   - Include code examples where relevant

3. **Cross-References**
   - Use relative links between documents
   - Reference specific versions in Git
   - Maintain a central index in the Wiki