# PetWalker Aggregate Documentation

## Overview
The PetWalker Aggregate represents a professional pet walker in the FurryFriends system. It encapsulates all information related to the walker's profile, services, certifications, and reputation.

## Key Components
- `PetWalker` (Aggregate Root)
- `ServiceArea` (Entity)
- `Photo` (Entity)
- `Testimonial` (Entity)
- `Schedule` (Entity)
- `Certification` (Entity)

## Value Objects
- `Name`
- `Email`
- `PhoneNumber`
- `Address`
- `Compensation`
- `GenderType`
- `Rating`

## Domain Events
- `PetWalkerCreated`
- `PetWalkerVerified`
- `ServiceAreaAdded`
- `CertificationAdded`
- `TestimonialReceived`
- `AvailabilityUpdated`
- `CompensationChanged`

## Business Rules
1. Pet walker must be at least 18 years old
2. First aid certification is required for verification
3. Insurance is required for active status
4. Maximum pets per walk must be between 1 and 5
5. Service areas must be within supported regions
6. Photos must include at least one profile picture
7. Hourly rate must be within acceptable range for region