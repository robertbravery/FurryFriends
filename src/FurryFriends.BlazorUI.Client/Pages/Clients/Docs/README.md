# FurryFriends Client Module Documentation

This folder contains diagrams that illustrate the flow of data, interactions between components, and architecture of the client functionality in the FurryFriends application.

## Available Diagrams

### Client Viewing

#### Client Pet View Sequence (`client-pet-view-sequence.mmd`)

This diagram maps the sequence of viewing a client and their pet details from the Clients page. It shows:

1. How the view action is initiated from the ClientList component
2. The role of the PopupService in managing popup state
3. How the ClientViewPopupManager handles popup display
4. The data loading process in the ClientViewPopup component
5. The API call to retrieve client and pet data
6. How the data flows back to the UI
7. The process of closing the popup

#### Client View Detailed Sequence (`client-view-detailed-sequence.mmd`)

This enhanced sequence diagram provides a more detailed view of the client viewing process, including:

1. The exact flow of data between all components
2. The role of the PetsViewDisplay component in showing pet information
3. State changes during the loading process
4. The complete event handling chain for opening and closing the popup

#### Pet View Flow (`pet-view-flow.mmd`)

This flowchart illustrates how pet information is displayed within the client view:

1. The loading and error handling process
2. Conditional display based on whether pets exist
3. The different elements of the pet cards (name, breed, photo, etc.)
4. The relationship between client information and pet display sections

### Client Creation

#### Client Creation Sequence (`client-creation-sequence.mmd`)

This sequence diagram illustrates the complete flow of creating a new client, including:

1. User interaction with the CreateClient component
2. Form validation process (client-side and server-side)
3. Data transformation and API communication
4. Domain validation in value objects
5. Business rule validation
6. Database persistence
7. Success and error handling

#### Client Creation Flow (`client-creation-flow.mmd`)

This flowchart provides a high-level overview of the client creation process, showing decision points and alternative paths for validation failures.

#### Client Creation Validation Flow (`client-creation-validation-flow.mmd`)

This specialized flowchart focuses on the multi-layered validation process during client creation:

1. Client-side validation using data annotations
2. Server-side command validation
3. Domain validation in value objects
4. Business rule validation

### Pet Management

#### Pet Creation Sequence (`pet-creation-sequence.mmd`)

This sequence diagram illustrates the process of adding a new pet to a client:

1. How the pet creation is initiated from the EditClientPopup
2. The role of the PetsDisplay component in triggering pet creation
3. The AddPetPopup component's initialization and form handling
4. The API communication for breed data and pet creation
5. The process of updating the UI after successful pet creation

#### Pet Update Sequence (`pet-update-sequence.mmd`)

This sequence diagram shows the process of updating an existing pet:

1. How pet editing is initiated from the PetsDisplay component
2. The EditPetPopup component's initialization with existing pet data
3. The validation and submission process
4. The API communication for updating pet information
5. The UI refresh process after successful update

#### Pet Creation Flow (`pet-creation-flow.mmd`)

This flowchart provides a high-level overview of the pet creation process:

1. The user interaction steps
2. Decision points for validation
3. The client-side and server-side processing
4. The UI update flow after successful creation

#### Pet Update Flow (`pet-update-flow.mmd`)

This flowchart illustrates the pet update process:

1. The steps to initiate pet editing
2. The validation and submission flow
3. Error handling paths
4. The UI refresh process

### Architecture Diagrams

#### C4 Context Diagram (`client-creation-c4-context.mmd`)

This diagram shows the FurryFriends system in context with its users and external systems.

#### C4 Container Diagram (`client-creation-c4-container.mmd`)

This diagram shows the high-level containers (applications, data stores) that make up the FurryFriends system.

#### C4 Component Diagram (Client Creation) (`client-creation-c4-component.mmd`)

This diagram decomposes the containers into components, showing how they interact during client creation.

#### C4 Component Diagram (Pet Management) (`pet-management-c4-component.mmd`)

This diagram shows the components involved in pet management functionality:

1. The UI components for displaying, adding, and editing pets
2. The API endpoints and handlers for pet operations
3. The domain entities and services for pet management
4. The data access components for persisting pet data

#### Class Diagram (Client Creation) (`client-creation-class-diagram.mmd`)

This diagram shows the relationships between the key classes involved in client creation, from UI models to domain entities.

#### Class Diagram (Pet Management) (`pet-management-class-diagram.mmd`)

This diagram illustrates the class relationships for pet management:

1. The UI components (EditClientPopup, PetsDisplay, AddPetPopup, EditPetPopup)
2. The data models (Pet, BreedDto)
3. The services for API communication
4. The relationships and dependencies between these classes

#### Comprehensive Client Domain Class Diagram (`client-domain-comprehensive-class-diagram.mmd`)

This detailed class diagram provides a complete view of the Client domain model and all its relationships:

1. Base classes and interfaces (EntityBase, AuditableEntity, UserEntityBase, IAggregateRoot, ValueObject)
2. Domain entities (Client, Pet, Breed, Species) with their properties and methods
3. Value objects (Name, Email, PhoneNumber, Address) with their validation logic
4. Enumerations (ClientType, ReferralSource, PetGender)
5. UI models (ClientModel, ClientDto, ClientData, PetModel, BreedDto)
6. All relationships between these classes (inheritance, composition, association)
7. Detailed notes explaining key architectural concepts

## Viewing the Diagrams

These diagrams are in Mermaid format (`.mmd` extension). You can view them using:

- GitHub (which natively renders Mermaid diagrams)
- VS Code with the Mermaid extension
- Online Mermaid Live Editor: https://mermaid.live/
- Any Markdown viewer that supports Mermaid syntax

## Diagram Conventions

### Sequence Diagrams
- Solid arrows (→) represent synchronous calls
- Dashed arrows (-->) represent asynchronous responses or events
- Notes are used to provide additional context
- Participants are arranged from left to right in order of their appearance in the flow

### Flow Diagrams
- Rectangles represent processes or actions
- Diamonds represent decision points
- Arrows show the flow direction
- Subgraphs group related activities

### C4 Diagrams
- Follow the standard C4 model notation (Context, Container, Component)
- Color coding is used to distinguish different types of relationships
- External systems are marked with dashed borders

### Class Diagrams
- Show relationships between classes (association, composition)
- Include key properties and methods
- Group related classes visually
