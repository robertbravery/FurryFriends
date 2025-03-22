# API Endpoints Documentation

## PetWalker Endpoints

### Create PetWalker
```http
POST /api/pet-walkers
Content-Type: application/json

{
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "phoneNumber": {
    "countryCode": "string",
    "number": "string"
  },
  "address": {
    "street": "string",
    "city": "string",
    "state": "string",
    "country": "string",
    "zipCode": "string"
  },
  "dateOfBirth": "string (yyyy-MM-dd)",
  "gender": "string",
  "biography": "string",
  "hourlyRate": {
    "amount": "decimal",
    "currency": "string"
  }
}
```

### Update Profile
```http
PUT /api/pet-walkers/{id}/profile
Content-Type: application/json

{
  "biography": "string",
  "maxPetsPerWalk": "number",
  "yearsOfExperience": "number",
  "hasInsurance": "boolean",
  "hasFirstAidCertification": "boolean"
}
```

### Add Service Area
```http
POST /api/pet-walkers/{id}/service-areas
Content-Type: application/json

{
  "zipCodes": ["string"],
  "radiusInMiles": "number"
}
```

### Update Availability
```http
PUT /api/pet-walkers/{id}/availability
Content-Type: application/json

{
  "schedule": [
    {
      "dayOfWeek": "number",
      "startTime": "string (HH:mm)",
      "endTime": "string (HH:mm)"
    }
  ]
}
```

### Get PetWalker Details
```http
GET /api/pet-walkers/{id}
Accept: application/json

Response:
{
  "id": "guid",
  "name": {
    "firstName": "string",
    "lastName": "string"
  },
  "email": "string",
  "phoneNumber": {
    "countryCode": "string",
    "number": "string"
  },
  "biography": "string",
  "rating": "number",
  "yearsOfExperience": "number",
  "serviceAreas": [
    {
      "zipCode": "string",
      "city": "string",
      "state": "string"
    }
  ],
  "testimonials": [
    {
      "id": "guid",
      "rating": "number",
      "comment": "string",
      "date": "string"
    }
  ]
}