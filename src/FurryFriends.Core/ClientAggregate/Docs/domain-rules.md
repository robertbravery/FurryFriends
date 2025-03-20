# Domain Rules and Invariants

## Client Rules

### Creation
1. A client must be created with:
   - Valid Name
   - Valid Email
   - Valid PhoneNumber
   - Valid Address
   - ClientType (defaults to Regular)
   - ReferralSource (defaults to None)

### Pet Management
1. A client can have multiple pets
2. Each pet must have:
   - Valid name
   - Valid breed
   - Age
   - Species
   - Weight
   - Color
   - Owner reference

### Contact Preferences
1. PreferredContactTime is optional
2. When provided, PreferredContactTime must be within business hours

## Pet Rules

### Creation
1. Each pet must be associated with exactly one client
2. Breed must match the specified species
3. Age must be non-negative
4. Weight must be positive

### Health Information
1. VaccinationStatus must be tracked
2. MedicalConditions are optional
3. DietaryRestrictions are optional

## Breed and Species Rules

### Species
1. Must have a unique name
2. Must have a description

### Breed
1. Must belong to exactly one species
2. Must have a unique name within its species
3. Must have a description