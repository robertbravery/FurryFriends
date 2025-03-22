# List PetWalkers Endpoint

## Flow Diagram

```mermaid
sequenceDiagram
    participant C as Client
    participant E as Endpoint
    participant M as Mediator
    participant H as Handler
    participant R as Repository
    
    C->>E: GET /api/pet-walkers
    E->>M: ListPetWalkersQuery
    M->>H: Handle Query
    H->>R: Get Walkers
    R-->>H: Walker List
    H-->>M: PetWalker DTOs
    M-->>E: Paged Result
    E-->>C: 200 OK
```

## Query Parameters
- page (default: 1)
- pageSize (default: 10)
- location (optional)
- maxHourlyRate (optional)
- availability (optional)
- hasInsurance (optional)
- isVerified (optional)

## Response Contract
```json
{
  "items": [
    {
      "id": "guid",
      "fullName": "string",
      "email": "string",
      "phoneNumber": "string",
      "city": "string",
      "locations": ["string"],
      "bioPicture": {
        "url": "string",
        "description": "string"
      },
      "photos": [
        {
          "url": "string",
          "description": "string"
        }
      ]
    }
  ],
  "totalItems": "number",
  "pageNumber": "number",
  "totalPages": "number"
}
```