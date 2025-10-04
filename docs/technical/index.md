# FurryFriends Technical Documentation

## Overview

This documentation provides comprehensive technical information about the FurryFriends application, a pet walking and day care service platform. It serves as the primary reference for developers working on the project.

## Architecture

- [Architecture Overview](2-architecture.md) - High-level architecture of the application
- [Design Decisions](3-design.md) - Key design decisions and patterns
- [Implementation Guide](4-implementation.md) - Code organization and standards

## Core Features

- [Client Management](../features/client-management.md) - Client profile and pet management
- [PetWalker Management](../features/petwalker-management.md) - PetWalker profile and service area management
- [Booking System](../features/booking-system.md) - Appointment booking and scheduling

## Technical Components

### Authentication and Security

- [Authentication and Authorization](authentication-authorization.md) - User authentication and access control
- [Security Best Practices](security-best-practices.md) - Security guidelines and implementation

### Logging and Monitoring

- [Logging Architecture](logging-architecture.md) - Overview of the logging system
- [Logging Implementation Details](logging-implementation-details.md) - Detailed implementation of logging
- [Logging Troubleshooting](logging-troubleshooting.md) - Common issues and solutions
- [Logging Security](logging-security.md) - Security considerations for logging
- [Logging Performance](logging-performance.md) - Performance optimization for logging

### Data Access

- [Data Access Layer](data-access.md) - Database access and ORM usage
- [Repository Pattern](repository-pattern.md) - Implementation of repositories
- [Entity Framework Core](entity-framework-core.md) - EF Core configuration and usage

### API Design

- [API Architecture](api-architecture.md) - RESTful API design principles
- [FastEndpoints](fastendpoints.md) - Usage of FastEndpoints framework
- [API Documentation](api-documentation.md) - API documentation with Swagger/OpenAPI

### UI Components

- [Blazor Components](blazor-components.md) - Reusable Blazor components
- [UI Design System](ui-design-system.md) - UI design guidelines and components
- [Client-Side State Management](client-side-state.md) - Managing state in Blazor WebAssembly

## Development Guides

- [Development Environment Setup](development-environment.md) - Setting up the development environment
- [Coding Standards](coding-standards.md) - Coding conventions and best practices
- [Testing Strategy](testing-strategy.md) - Unit, integration, and end-to-end testing
- [Debugging Guide](debugging-guide.md) - Debugging techniques and tools

## Deployment and Operations

- [Deployment Guide](deployment-guide.md) - Deploying the application to production
- [Configuration Management](configuration-management.md) - Managing application configuration
- [Monitoring and Alerting](monitoring-alerting.md) - Monitoring the application in production
- [Performance Optimization](performance-optimization.md) - Optimizing application performance

## Appendices

- [Glossary](glossary.md) - Definitions of key terms
- [External Dependencies](external-dependencies.md) - Third-party libraries and services
- [Troubleshooting Common Issues](troubleshooting.md) - Solutions to common problems

## How to Use This Documentation

1. **New Developers**: Start with the [Architecture Overview](2-architecture.md) to understand the high-level structure, then explore the [Core Features](#core-features) to understand the main functionality.

2. **Feature Developers**: Refer to the specific feature documentation in the [Core Features](#core-features) section, along with the relevant technical components.

3. **DevOps Engineers**: Focus on the [Deployment and Operations](#deployment-and-operations) section for information on deploying and maintaining the application.

4. **QA Engineers**: Review the [Testing Strategy](testing-strategy.md) and [Debugging Guide](debugging-guide.md) for information on testing and troubleshooting.

## Contributing to Documentation

When contributing to this documentation:

1. Follow the established format and structure
2. Use clear, concise language
3. Include diagrams where appropriate (using Mermaid syntax)
4. Provide code examples for technical concepts
5. Update the relevant index files when adding new documentation

For more information on contributing, see the [Documentation Guidelines](documentation-guidelines.md).
