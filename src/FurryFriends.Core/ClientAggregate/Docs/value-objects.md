# Value Objects

## Name
Represents a person's name in the system.

### Rules
- Cannot be null or empty
- Must contain at least first and last name
- Maximum length restrictions apply

## Email
Represents an email address.

### Rules
- Must be a valid email format
- Cannot be null or empty
- Must be unique in the system

## PhoneNumber
Represents a contact phone number.

### Rules
- Must include country code
- Must be in valid format
- Cannot be null or empty

## Address
Represents a physical address.

### Rules
- Must include street, city, state/province, and postal code
- Country is required
- Postal code must be in valid format for the country