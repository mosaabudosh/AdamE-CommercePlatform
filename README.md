# ArabazonPlatform.Core

A modular e-commerce platform backend built with **ASP.NET Core 8**, **Entity Framework Core**, and **SQL Server**.

## Overview

**ArabazonPlatform.Core** is a modular backend solution designed to support e-commerce business operations through a structured and maintainable architecture.

The solution separates API endpoints, application workflows, domain logic, infrastructure services, and data access into dedicated projects.

It also includes shared components and reporting capabilities to support the platform's business requirements.

## Technology Stack

| Category | Technology |
|---|---|
| Framework | .NET 8 |
| Backend | ASP.NET Core Web API |
| Language | C# |
| Database | Microsoft SQL Server |
| ORM | Entity Framework Core 8 |
| Data Access | Dapper |
| Application Mediation | MediatR |
| Authentication | JWT |
| Real-time Communication | SignalR |
| API Documentation | Swagger / OpenAPI |
| Reporting | ReportViewerCore, RDLC |
| PDF Processing | PDFsharp |
| QR Code Generation | QRCoder |
| JSON Processing | Newtonsoft.Json |

## Solution Architecture

The solution is organized into separate projects to improve maintainability and separation of concerns.

| Project | Responsibility |
|---|---|
| `ArabazonPlatform.Core.API` | Web API endpoints and application entry point |
| `ArabazonPlatform.Core.Application` | Application workflows, services, and request handling |
| `ArabazonPlatform.Core.Domain` | Core domain models and business logic |
| `ArabazonPlatform.Core.Infrastructure` | Infrastructure services and external integrations |
| `ArabazonPlatform.Core.Infrastructure.Data` | Database access and persistence components |
| `ArabazonPlatform.Generic` | Shared components and reusable utilities |
| `ArabazonPlatform.Core.ReportsDesigner` | Typed DataSets and report definitions |
| `DDLs` | Database definition scripts and supporting SQL files |

## Key Technical Components

- **RESTful APIs:** Backend endpoints for platform operations.
- **Authentication:** JWT-based authentication support.
- **Data Access:** Entity Framework Core and Dapper for SQL Server data operations.
- **Application Workflows:** MediatR for request handling and application-level separation.
- **Real-time Communication:** SignalR integration.
- **API Documentation:** Swagger / OpenAPI.
- **Reporting:** RDLC report definitions and ReportViewerCore.
- **PDF and QR Utilities:** PDF generation and QR code functionality.

## Prerequisites

Before running the solution, make sure you have:

- .NET 8 SDK
- Visual Studio 2022 or a compatible .NET development environment
- Microsoft SQL Server
- A configured database connection

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/mosaabudosh/ArabazonPlatform.Core.git
```

### 2. Navigate to the Project Directory

```bash
cd ArabazonPlatform.Core
```

### 3. Restore Dependencies

```bash
dotnet restore ArabazonPlatform.Core.sln
```

### 4. Configure the Database

Configure the SQL Server connection string using the configuration mechanism already defined by the API project.

Use your local development configuration or .NET User Secrets for sensitive values.

Do not commit database passwords, JWT secrets, or other credentials to source control.

### 5. Prepare the Database

Prepare the database using the project's existing Entity Framework migrations and/or the required SQL scripts provided in the repository.

Make sure the database schema is compatible with the application before starting the API.

### 6. Build the Solution

```bash
dotnet build ArabazonPlatform.Core.sln --configuration Release
```

### 7. Run the API

```bash
dotnet run --project ArabazonPlatform.Core.API/ArabazonPlatform.Core.API.csproj
```

Once the API is running, use the Swagger endpoint configured by the application to explore the available API endpoints.

## Configuration and Security

- Keep connection strings and credentials outside source control.
- Use separate configuration values for development and production environments.
- Store JWT signing secrets securely.
- Review CORS settings before deployment.
- Avoid committing local IDE files, build outputs, temporary files, or sensitive data.

## Development Notes

- The solution targets .NET 8.
- Database operations rely on SQL Server.
- Reporting components use RDLC definitions and typed DataSets.
- Keep report DataSet names synchronized with the names supplied by the application.
- Verify database configuration before running the application in a new environment.

## License

No license has been specified yet. Add a `LICENSE` file before distributing the project under an open-source license.