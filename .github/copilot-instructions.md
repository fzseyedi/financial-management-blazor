# GitHub Copilot Instructions for Blazor WebApp (Minimal API, CQRS, MediatR)

This file provides context and instructions to GitHub Copilot for working with this Blazor WebApp project, which utilizes Minimal APIs and the CQRS pattern with MediatR.

## Project Overview

This is a Blazor WebApp Auto render mode application that interacts with a backend built using ASP.NET Core Minimal APIs. CRUD operations on the backend are implemented following the CQRS (Command Query Responsibility Segregation) pattern using the MediatR library.

## Architecture Principles

- **Clean Architecture:**
    - **Domain Layer:** Contains core business entities, value objects, and domain events. It has no dependencies on other layers.
    - **Application Layer:** Contains application-specific business logic and use cases. It depends only on the Domain layer. This is where Commands, Queries, and their Handlers typically reside. Interfaces for infrastructure concerns are defined here.
    - **Infrastructure Layer:** Contains implementations of external concerns defined in the Application layer (e.g., database access with Entity Framework Core, external services, file system interaction). It depends on the Domain and Application layers.
    - **Presentation Layer (API):** The entry point for external interactions (e.g., Minimal APIs). It depends on the Application and Infrastructure (for dependency injection setup).
    - Dependencies flow inwards, with the core (Domain) being independent.

- **Vertical Slice Architecture:**
    - Code is organized primarily by feature. A "slice" represents a complete feature, encompassing all necessary components across layers (e.g., a specific Command/Query, its Handler, related validation, and potentially relevant data access logic or DTOs).
    - This approach minimizes coupling *between* features and keeps feature-related code together, making it easier to understand, develop, and maintain individual features.

## Technology Stack

- **Frontend:** Blazor WebApp Auto render mode (.NET)
- **Backend:** ASP.NET Core Minimal APIs (.NET)
- **Language:** C#, HTML, CSS, JavaScript (for interop if used)
- **UI Framework:** Syncfusion
- **State Management:** typical Blazor patterns
- **Architecture:** CQRS (Command Query Responsibility Segregation)
- **Messaging/Dispatch:** MediatR
- **Data Access:** Entity Framework Core

## Coding Conventions and Best Practices

- Follow standard C# naming conventions (PascalCase for classes, methods, properties; camelCase for parameters and local variables).
- Use asynchronous programming with `async` and `await` for I/O bound operations, especially in component lifecycle methods like `OnInitializedAsync` and in API handlers.
- Prefer using `ConfigureAwait(false)` when awaiting tasks in library code or non-UI contexts.
- Components should be kept focused and reusable where possible.
- Use `@bind` for two-way data binding in components.
- Inject dependencies using the `@inject` directive or `[Inject]` attribute in Blazor components, and via constructor injection or method parameters in backend services/handlers.
- Handle exceptions gracefully on both the frontend (Blazor) and backend (Minimal APIs, middleware).
- Write clear and concise code with appropriate comments where necessary, particularly explaining the purpose of Commands, Queries, and Handlers.
- Ensure proper disposal of disposable resources (`IDisposable`).
- Structure the backend code logically, grouping related Commands, Queries, and Handlers, often by feature.

## Backend (Minimal API, CQRS, MediatR) Specific Instructions

**Clean Architecture Layers:**
    - When generating code, be mindful of the layer in which the code resides and its allowed dependencies.
    - Domain entities should be simple and free of infrastructure or application logic.
    - Application layer code should define *what* needs to be done (use cases via Commands/Queries and Handlers), while Infrastructure handles *how*.
    - The Presentation layer should focus on UI or API concerns and interact with the Application layer.

- **Vertical Slices:**
    - When creating new features, organize related files (Command, Query, Handler, Validator, DTOs) together, typically within the Application layer, in a folder named after the feature (e.g., `Application/Features/Products/Commands/CreateProduct`).
    - Minimize dependencies *between* Vertical Slices. Reusable logic should be extracted into shared components, often in the Application or Domain layers.

- **Minimal API:**
    - Endpoints are defined in the Presentation layer (e.g., `Api/Program.cs` or endpoint-specific files).
    - Endpoints should inject `IMediator` and dispatch Commands or Queries received from the HTTP request.
    - Map HTTP methods (GET, POST, PUT, DELETE) to the appropriate Query or Command.

- **CQRS with MediatR:**
    - Commands should represent actions that change state.
    - Queries should represent requests for data.
    - Handlers implement `IRequestHandler<TRequest, TResponse>` and contain the core logic for a specific Command or Query. They are typically located in the Application layer, within their feature's vertical slice.
    - Use MediatR's `Send` method in the Presentation layer to dispatch requests to the Application layer Handlers.

- For CRUD operations:
    - Create, Update, and Delete actions on the frontend will send **Commands** to the backend Minimal API endpoints.
    - Read actions (fetching data) on the frontend will send **Queries** to the backend Minimal API endpoints.
    - Ensure the API endpoints correctly map HTTP methods (POST, PUT, DELETE, GET) to the appropriate Commands or Queries.
- When suggesting backend file structure, follow a pattern that groups Commands, Queries, and Handlers logically (e.g., by feature or by Commands/Queries folders).

## Blazor Frontend Interaction with Backend

- When suggesting Blazor code for interacting with the backend, use `HttpClient` to send HTTP requests to the Minimal API endpoints.
- Map Blazor component actions (button clicks, form submissions) to sending the appropriate Commands or Queries to the backend via `HttpClient`.
- Handle the responses from the Minimal APIs, including successful results and error states.
- Consider using Data Transfer Objects (DTOs) that are specifically designed for the frontend, even if they are similar to the Command/Query objects used on the backend, to maintain separation of concerns.

## Specific Project Details 

- The application uses a `AuthStateProvider` for authentication. When suggesting authentication-related code, consider how it integrates with API calls (e.g., attaching tokens).
- API endpoints typically follow a convention like `/api/[FeatureName]/[Operation]`.
- Project

