# Validation Rules

## Client Validation

### Name Validation
- First Name:
  - Required
  - Length: 2-50 characters
  - Allowed characters: letters, hyphens, apostrophes
- Last Name:
  - Required
  - Length: 2-50 characters
  - Allowed characters: letters, hyphens, apostrophes

### Contact Information
- Email:
  - Required
  - Must be valid format
  - Must be unique in system
  - Maximum length: 255 characters
- Phone:
  - Required
  - Valid country code
  - Valid number format for country
  - Maximum length: 15 digits (excluding country code)

### Address
- Street:
  - Required
  - Length: 5-100 characters
- City:
  - Required
  - Length: 2-50 characters
- State/Province:
  - Required
  - Length: 2-50 characters
- Postal Code:
  - Required
  - Format validation per country
- Country:
  - Required
  - Must be valid ISO country code

## Pet Validation

### Basic Information
- Name:
  - Required
  - Length: 2-50 characters
- Age:
  - Required
  - Non-negative number
- Weight:
  - Required
  - Positive number
  - Maximum: 1000 kg

### Breed and Species
- Breed:
  - Required
  - Must exist in system
  - Must match selected species
- Species:
  - Required
  - Must exist in system

### Health Information
- Vaccination Status:
  - Required boolean
- Medical Conditions:
  - Optional
  - Maximum length: 1000 characters
- Dietary Restrictions:
  - Optional
  - Maximum length: 500 characters

## Business Rules Validation

### Client Type Rules
- Regular:
  - Basic validation only
- Premium:
  - Requires valid contact information
  - Preferred contact time must be set
- Corporate:
  - Requires business address
  - Requires business contact information

### Pet Limits
- Regular: Maximum 3 pets
- Premium: Maximum 5 pets
- Corporate: No limit

### Contact Time
- Must be between 8:00 AM and 6:00 PM local time
- Must be on the hour or half hour