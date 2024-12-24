### **Overview Document**

#### **1. Purpose and Scope of the Application**

**Purpose**:
The purpose of the Pet Walking/Day Care App is to create a platform that connects pet owners with reliable and trusted pet walkers and day care providers. The application aims to provide a seamless, user-friendly experience that ensures the safety and well-being of pets while offering convenience and peace of mind for pet owners. This service will cater to the growing need for professional pet care services in urban areas.

**Scope**:
The application will feature two primary user roles: Pet Walkers/Day Care Providers and Clients (Pet Owners). The scope includes registration, profile management, search and discovery, booking system, payment integration, rating and review system, and in-app communication. The app will be accessible via web browsers and mobile devices, ensuring availability and convenience for users across platforms. The backend will be built using the .NET stack with Clean Architecture and vertical slices, leveraging MS SQL for data storage and Azure App Services for hosting and deployment.

#### **2. High-Level Architecture Diagram**

Below is a high-level architecture diagram illustrating the main components and their interactions:

```
+---------------------+                        +---------------------+
|  Blazor WebAssembly | <--------->  |   ASP.NET Core Web API   |
|     (Web App)       |                        |      (Backend)            |
+---------------------+                        +---------------------+
          |
          |
+--------------------+          +-----------------------+          +-------------+
| Xamarin / .NET MAUI  | <--------->  |  MS SQL Server  | <---------> | Azure App |
|    (Mobile App)         |                          |     (Database)    |         |  Services  |
+--------------------+                          +-----------------------+          +-------------+

```

#### **3. Description of Layers and Components**

**Frontend Layer**:
- **Blazor WebAssembly**: This is used for creating the web application. It offers a single-page application (SPA) experience with rich interactivity and performance, running on the client side.
- **Xamarin / .NET MAUI**: This is used for creating mobile applications for iOS and Android. It allows code sharing between the web and mobile apps, providing a native experience on mobile devices.

**Backend Layer**:
- **ASP.NET Core Web API**: This serves as the middle layer handling all business logic, data processing, and communication with the database. It is designed using Clean Architecture with vertical slices to ensure separation of concerns, testability, and maintainability.
  - **Vertical Slices**: Each feature or functionality of the app is encapsulated into its own slice, including commands, queries, handlers, validators, and endpoints. This modular approach enhances code organization and scalability.
  - **FastEndpoints**: A lightweight framework used to define endpoints, making the API more intuitive and easier to maintain.
  - **Fluent Validation**: Utilized for validating requests to ensure data integrity and consistency.

**Database Layer**:
- **MS SQL Server**: This relational database management system stores all structured data, including user information, pet details, booking records, and transaction histories. Entity Framework Core is used for object-relational mapping (ORM) to interact with the database.

**Hosting and Deployment**:
- **Azure App Services**: This platform hosts the web application, API, and database, providing features such as scalability, automatic updates, and robust security. Azure App Services ensures high availability and performance for the application.

**DevOps**:
- **Azure DevOps**: This is used for continuous integration and continuous deployment (CI/CD). It automates the process of building, testing, and deploying applications, ensuring that updates and new features are delivered efficiently and reliably.

**Security**:
- **Authentication & Authorization**: Implemented using Azure Active Directory or IdentityServer to secure user access and manage user identities.
- **Data Protection**: Encryption is used for sensitive data at rest and in transit to ensure data security and privacy.
- **API Security**: OAuth 2.0 and HTTPS are implemented for secure API communication, protecting data exchanges between the client and server.
