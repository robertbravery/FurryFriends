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

### Architecture Diagrams

#### C4 Context Diagram (`client-creation-c4-context.mmd`)

This diagram shows the FurryFriends system in context with its users and external systems.

#### C4 Container Diagram (`client-creation-c4-container.mmd`)

This diagram shows the high-level containers (applications, data stores) that make up the FurryFriends system.

#### C4 Component Diagram (`client-creation-c4-component.mmd`)

This diagram decomposes the containers into components, showing how they interact during client creation.

#### Class Diagram (`client-creation-class-diagram.mmd`)

This diagram shows the relationships between the key classes involved in client creation, from UI models to domain entities.

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
