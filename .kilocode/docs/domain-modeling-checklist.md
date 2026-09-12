# 🐾 FurryFriends Domain Modeling Checklist
*A complete, architecture‑aligned checklist for modeling domain concepts in the FurryFriends system.*

---

## 🧩 1. Ubiquitous Language & Domain Concepts

Domain models must reflect real business concepts used by:

- Pet Owners  
- Walkers  
- Pets  
- Scheduling  
- Ratings  
- Bookings  

### ✔ Checklist
- Names match the language in:
  - `spec.md`
  - `plan.md`
  - `constitution.md`
  - Feature documentation
- No infrastructure or API terminology in the domain
- No accidental complexity or technical naming
- Concepts map directly to real-world meaning

---

## 🧩 2. Aggregate Boundaries

Aggregates must be:

- Small  
- Cohesive  
- Invariant‑protecting  
- Transactionally consistent  

### ✔ Checklist
- Aggregate has **one clear root**
- All mutations go through the root
- Child entities are never modified directly
- Invariants are enforced **immediately** when state changes
- No cross‑aggregate references
- No EF Core navigation properties
- No lazy loading
- No external service calls inside aggregates

### Examples of good aggregates
- `Pet`
- `Walker`
- `Booking`
- `Rating`
- `Schedule`

---

## 🧩 3. Entity Rules (FurryFriends Constitution)

Entities must be:

- Immutable except via methods  
- Behavior‑rich  
- Free of EF Core attributes  
- Free of infrastructure concerns  

### ✔ Checklist
- Properties are private set or constructor‑set
- Mutations occur through **methods**, not setters
- Entities expose **behavior**, not just data
- Domain events raised when meaningful changes occur
- Invariants enforced inside the entity

---

## 🧩 4. Value Objects

Value objects must be:

- Immutable  
- Self‑validating  
- Comparable by value  
- Free of identity  

### ✔ Checklist
- Constructor validates all invariants
- Equality is value‑based
- No identity or persistence concerns
- Represents a domain concept, not a data bag

### Examples
- `PetName`
- `OwnerEmail`
- `WalkerRating`
- `BookingTimeRange`
- `GeoLocation`

---

## 🧩 5. Domain Services

Domain services must be:

- Stateless  
- Pure  
- Focused on domain logic that doesn’t belong to an entity  

### ✔ Checklist
- Logic truly doesn’t belong to an entity or aggregate
- Service is stateless
- No infrastructure concerns
- No EF Core usage
- No external API calls

### Examples
- `BookingPolicyService`
- `WalkerAvailabilityService`

---

## 🧩 6. Factories

Factories create complex aggregates.

### ✔ Checklist
- Encapsulates complex creation logic
- Enforces invariants
- Avoids infrastructure
- Avoids EF Core

---

## 🧩 7. Domain Events

Domain events must be:

- Meaningful  
- Business‑driven  
- Raised inside aggregates  

### ✔ Checklist
- Represents a real business occurrence
- Raised inside the aggregate root
- No infrastructure concerns
- No DTOs or external types

### Examples
- `PetRegistered`
- `BookingCreated`
- `WalkerRated`

---

## 🧩 8. Repositories

Repositories belong to the **Application layer**, but must follow domain rules.

### ✔ Checklist
- Returns domain aggregates, not EF Core entities
- Avoids returning IQueryable
- Avoids leaking persistence concerns
- Avoids infrastructure types in signatures

---

## 🧩 9. CQRS Alignment

Commands mutate state.  
Queries never mutate state.

### ✔ Checklist

**Command Handlers**
- Load aggregate  
- Call domain methods  
- Persist changes  
- Publish domain events  
- No business logic  

**Query Handlers**
- Only read  
- Never mutate  
- Return DTOs, not domain entities  

---

## 🧩 10. Layer Boundaries (FurryFriends Constitution)

**Domain layer must NOT reference:**

- EF Core  
- HTTP  
- Controllers  
- DTOs  
- Infrastructure  
- Logging  
- External services  

### ✔ Checklist
- No EF Core attributes  
- No DI attributes  
- No DTOs  
- No external service calls  
- No static service locators  
- No configuration access  

---

## 🧩 11. Validation Rules

Validation must be:

- In domain entities (invariants)  
- In value objects (self‑validation)  
- In API layer (request validation)  

### ✔ Checklist
- Invariants enforced in domain  
- Request validations done in API  
- Business rules enforced in domain methods  

---

## 🧩 12. Constitution Compliance

This is the “final boss” of domain modeling.

### ✔ Checklist
- All constitution rules followed  
- Responsibilities correctly placed  
- Boundaries respected  
- Aggregates small and cohesive  
- Invariants enforced  
- Domain events meaningful  
- Value objects used correctly  
- Entities behavior‑rich  

---

## 🧩 13. Feature Folder Alignment (SpecKit Integration)

Domain models must align with the active feature folder:

- `.specify/feature.json`  
- `<feature_dir>/spec.md`  
- `<feature_dir>/plan.md`  
- `<feature_dir>/tasks.md`  

### ✔ Checklist
- Domain model matches the active feature’s spec  
- Plan.md describes the domain responsibilities clearly  
- Tasks.md reflect domain operations and invariants  
- No domain logic outside the feature’s scope  

---

## 🧩 14. Review Readiness

Domain models must be ready for:

- Multi‑agent correctness review  
- Security review  
- Constitution compliance review  
- Aggregate review  
- SpecKit feature validation  

### ✔ Checklist
- Domain logic is isolated and testable  
- No infrastructure leakage  
- No accidental complexity  
- Clear invariants  
- Clear aggregate boundaries  
- Clear domain events  

---
