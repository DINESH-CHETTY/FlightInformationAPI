# Flight Information API

A RESTful API for managing flight information built with .NET 8, following clean architecture principles and best practices.

## Features

- Complete CRUD operations for flights
- Flight search functionality with multiple criteria
- Input validation using FluentValidation
- Comprehensive logging with NLog
- Unit and integration tests
- Swagger/OpenAPI documentation
- Global exception handling
- In-memory database for simplicity

## Technologies Used

- .NET 8
- Entity Framework Core (In-Memory)
- FluentValidation
- NLog for logging
- xUnit for testing
- Moq for mocking
- FluentAssertions for test assertions
- Swagger/OpenAPI for documentation

## Architecture

This project follows Clean Architecture principles with the following layers:

- **Core**: Contains entities, DTOs, interfaces, and business rules
- **Infrastructure**: Contains data access, external services implementations
- **API**: Contains controllers, middleware, and configuration
- **Tests**: Contains unit and integration tests

## Getting Started

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or VS Code

### Installation
 Clone the repository:
```bash
git clone https://github.com/DINESH-CHETTY/FlightInformationAPI.git

### Running the Application
### Commands
```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the API
dotnet run --project FlightInformationApi

# Run tests
dotnet test

### Access Points
API: https://localhost:7137
Swagger UI: https://localhost:7137/swagger
