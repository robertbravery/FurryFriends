# PetWalker Endpoints Architecture

## Overview

The PetWalker endpoints implement a RESTful API using FastEndpoints, following CQRS pattern with MediatR.

```mermaid
graph TD
    subgraph API Layer
        E[FastEndpoints]
        V[FluentValidation]
        M[Mappers]
        R[DTOs]
    end
    
    subgraph Application Layer
        CMD[Commands]
        QRY[Queries]
        MED[MediatR]
    end
    
    subgraph Domain Layer
        AGG[PetWalker Aggregate]
        VAL[Value Objects]
        EVT[Domain Events]
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
    AGG --> EVT
```

## Endpoint Routes

| Method | Route | Description | Auth Required |
|--------|-------|-------------|---------------|
| POST | /api/pet-walkers | Create new walker | No |
| PUT | /api/pet-walkers/{id}/profile | Update profile | No |
| PUT | /api/pet-walkers/{id}/hourly-rate | Update rate | No |
| POST | /api/pet-walkers/{id}/service-areas | Add service area | No |
| PUT | /api/pet-walkers/{id}/availability | Update availability | No |
| GET | /api/pet-walkers/{id} | Get walker details | No |
| GET | /api/pet-walkers | List walkers | No |
| DELETE | /api/pet-walkers/{id} | Delete walker | No |

## State Machine

```mermaid
stateDiagram-v2
    [*] --> Pending: Create Account
    Pending --> UnderReview: Submit Documents
    UnderReview --> Verified: Pass Verification
    UnderReview --> Rejected: Fail Verification
    Verified --> Active: Complete Profile
    Active --> Suspended: Policy Violation
    Active --> Inactive: Deactivate Account
    Suspended --> Active: Review Complete
    Inactive --> Active: Reactivate
    Rejected --> UnderReview: Resubmit
```