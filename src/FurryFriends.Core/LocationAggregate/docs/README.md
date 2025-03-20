# Location Aggregate Documentation

## Overview
The Location Aggregate manages geographical hierarchies in the FurryFriends system, organizing locations into Countries, Regions, and Localities. This structure supports service area definitions for pet walkers and enables efficient geographical searches.

## Key Components
- `Country` (Aggregate Root)
- `Region` (Entity)
- `Locality` (Entity)

## Value Objects
- `Coordinates` (Latitude/Longitude)
- `PostalCode`
- `Distance`

## Domain Events
- `CountryCreated`
- `RegionAdded`
- `LocalityAdded`
- `LocalityMerged`
- `RegionRenamed`

## Relationships
- Country ─┬─> Region ─> Locality
           └─> PostalCodes

## Business Rules
1. Each Locality must belong to exactly one Region
2. Each Region must belong to exactly one Country
3. Locality names must be unique within a Region
4. Region names must be unique within a Country
5. Deletion of a Country/Region is only allowed if no ServiceAreas reference it