# Client Aggregate Documentation

## Overview
The Client Aggregate represents a pet owner in the FurryFriends system. It serves as the aggregate root for managing client information, their pets, and related data.

## Key Components
- `Client` (Aggregate Root)
- `Pet` (Entity)
- `Species` (Entity)
- `Breed` (Entity)

## Value Objects
- `Name`
- `Email`
- `PhoneNumber`
- `Address`

## Enums
- `ClientType`: Regular, Premium, Corporate
- `ReferralSource`: None, Website, ExistingClient, SocialMedia, SearchEngine, Veterinarian, LocalAdvertising, Other

## Domain Rules
1. Every client must have a valid name, email, phone number, and address
2. Clients can have multiple pets
3. Each pet must belong to exactly one client
4. Pets must have a valid breed which belongs to a species