# ADR-002: Domain Folder Structure Convention

## Status

Accepted

## Context

As the FurryFriends application grew, we noticed that domain-related use case code (commands, queries, handlers) was being placed inconsistently. Initially, the Ratings use cases were placed directly under `src/FurryFriends.UseCases/Rating/` while other aggregates like Bookings, Clients, and PetWalkers correctly resided under `src/FurryFriends.UseCases/Domain/`.

This inconsistency violated Domain-Driven Design principles by not maintaining clear boundaries between domain logic and application infrastructure.

## Decision

All domain-related use case code must reside under `src/FurryFriends.UseCases/Domain/` following the established pattern:

- `src/FurryFriends.UseCases/Domain/Bookings/`
- `src/FurryFriends.UseCases/Domain/Clients/`
- `src/FurryFriends.UseCases/Domain/PetWalkers/`
- `src/FurryFriends.UseCases/Domain/Ratings/` (after moving)

Each aggregate folder should contain its own substructure for Commands, Queries, DTOs, etc.

## Consequences

### Positive

- Clear separation of concerns between domain logic and infrastructure
- Consistent organization makes code easier to locate
- Better alignment with DDD principles
- Improved maintainability as the project grows

### Negative

- Requires updating existing references when moving code
- Slightly longer namespace paths

## Implementation

When creating new domain features:

1. Create the feature folder under `src/FurryFriends.UseCases/Domain/{FeatureName}/`
2. Use namespace `FurryFriends.UseCases.Domain.{FeatureName}.{Subfolder}`
3. Follow the existing structure patterns from Bookings, Clients, or PetWalkers

Existing code has been migrated to follow this pattern (see commit moving Ratings to Domain/Ratings).
