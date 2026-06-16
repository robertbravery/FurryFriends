# Login API Contract

## Endpoint
POST `/api/auth/login`

## Request
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

## Response (200 OK)
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2026-06-16T15:00:00Z",
    "role": "Client"
  }
}
```

## Response (401 Unauthorized)
```json
{
  "success": false,
  "message": "Invalid email or password",
  "errors": ["Invalid credentials"]
}
```

## Response (400 Bad Request)
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": ["Email is required", "Password is required"]
}
```