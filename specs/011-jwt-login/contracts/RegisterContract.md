# Register API Contract

## Endpoint
POST `/api/auth/register`

## Request
```json
{
  "email": "newuser@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!",
  "role": "Client",
  "profileDetails": {
    "firstName": "John",
    "lastName": "Doe",
    "phoneNumber": "555-1234",
    "address": {
      "street": "123 Main St",
      "city": "Springfield",
      "state": "IL",
      "postalCode": "62701",
      "country": "USA"
    }
  }
}
```

## Response (201 Created)
```json
{
  "success": true,
  "data": {
    "userId": "929ccaf2-8c74-49bb-b9a0-ce26db0611ab",
    "role": "Client",
    "profileCreated": true,
    "message": "Registration successful. Please login."
  }
}
```

## Response (400 Bad Request)
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": [
    "Email is required",
    "Password must be at least 8 characters with uppercase, lowercase, number, and special character",
    "Role must be Client or Petwalker"
  ]
}
```

## Response (409 Conflict)
```json
{
  "success": false,
  "message": "User already exists",
  "errors": ["Email already registered"]
}
```