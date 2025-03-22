# Client Use Cases Documentation

## Overview
This module contains all application-level use cases for managing clients in the FurryFriends system. It follows CQRS pattern, separating read and write operations into Commands and Queries.

## Structure
```
Domain/Clients/
├── Command/
│   ├── CreateClient/
│   ├── UpdateClient/
│   ├── AddPet/
│   └── UpgradeClient/
├── Query/
│   ├── GetClient/
│   ├── ListClients/
│   └── SearchClients/
└── Common/
    ├── DTOs/
    └── Validators/
```

## Key Components
- Commands: Write operations (Create, Update, Delete)
- Queries: Read operations (Get, List, Search)
- DTOs: Data transfer objects for client operations
- Validators: Command and query validation rules
- Handlers: Command and query handlers

## Dependencies
- Core Layer: `FurryFriends.Core.ClientAggregate`
- Infrastructure: Through abstractions only