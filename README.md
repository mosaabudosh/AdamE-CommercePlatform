# ArabazonPlatform.Core

### Multi-Vendor E-Commerce Platform

**ArabazonPlatform.Core** is the core backend solution of a Multi-Vendor E-Commerce Platform, built using **ASP.NET Core 8**, **C#**, and **Microsoft SQL Server**.

The solution is organized into modular projects to separate API responsibilities, application workflows, domain logic, infrastructure services, database access, and reporting components.

It includes authentication and authorization capabilities, data access components, real-time communication, and reporting utilities.

---

## Technology Stack

| Category | Technology |
|---|---|
| Framework | .NET 8 |
| Backend | ASP.NET Core Web API |
| Programming Language | C# |
| Database | Microsoft SQL Server |
| ORM | Entity Framework Core 8 |
| Data Access | Dapper |
| Request Handling | MediatR |
| Authentication | JWT |
| Authorization | Role-Based Access Control and Permissions |
| Real-Time Communication | SignalR |
| API Documentation | Swagger / OpenAPI |
| Reporting | ReportViewerCore and RDLC |
| PDF Processing | PDFsharp |
| QR Code Generation | QRCoder |
| JSON Processing | Newtonsoft.Json |

---

## Solution Structure

The solution is divided into the following projects:

| Project | Description |
|---|---|
| `ArabazonPlatform.Core.API` | ASP.NET Core Web API and application entry point |
| `ArabazonPlatform.Core.Application` | Application services and request handling |
| `ArabazonPlatform.Core.Domain` | Domain models and business logic |
| `ArabazonPlatform.Core.Infrastructure` | Infrastructure services and integrations |
| `ArabazonPlatform.Core.Infrastructure.Data` | Database access and persistence components |
| `ArabazonPlatform.Generic` | Shared components and reusable utilities |
| `ArabazonPlatform.Core.ReportsDesigner` | Typed DataSets and RDLC report definitions |
| `DDLs` | Database definition scripts and supporting SQL files |

---

## Main Technical Capabilities

### Authentication and Authorization

- JWT-based authentication.
- Role-based authorization.
- Permission management.

### Data Access and Persistence

- Entity Framework Core for ORM and database operations.
- Dapper for lightweight SQL data access.
- SQL Server database integration.

### Application Architecture

- Modular project organization.
- Separation of concerns.
- MediatR-based request handling.
- Shared and reusable components.

### Real-Time Communication

- SignalR integration.

### Reporting and Document Utilities

- RDLC report definitions.
- ReportViewerCore integration.
- PDF processing using PDFsharp.
- QR code generation using QRCoder.

### API Documentation

- Swagger / OpenAPI integration.

---

## Prerequisites

Before running the solution, install:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/)
- Microsoft SQL Server
- Git

---

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

Configure the SQL Server connection string using the configuration settings expected by the API project.

Use local development settings or .NET User Secrets to store sensitive configuration values.

Do not commit production credentials, database passwords, or JWT signing secrets.

### 5. Prepare the Database

Prepare the database using the existing Entity Framework migrations and/or the SQL scripts provided in the solution.

Ensure that the configured database is compatible with the application before running the API.

### 6. Build the Solution

```bash
dotnet build ArabazonPlatform.Core.sln --configuration Release
```

### 7. Run the API

```bash
dotnet run --project ArabazonPlatform.Core.API/ArabazonPlatform.Core.API.csproj
```

Once the API is running, access the Swagger endpoint configured by the application.

---

## Security Considerations

- Keep sensitive configuration values outside source control.
- Use secure JWT signing keys.
- Apply appropriate authorization rules to protected endpoints.
- Review CORS settings before deployment.
- Avoid committing real customer data or production credentials.

---

## Testing

Automated Unit Test and Integration Test projects are not currently included in this solution.

The solution has been built successfully in Release configuration in the development environment.

---

## Future Improvements

Potential areas for further improvement include:

- Adding automated unit and integration tests.
- Adding automated CI build and validation workflows.
- Improving developer setup and deployment documentation.
- Reviewing package dependencies and security updates.

---

## Repository

GitHub: [ArabazonPlatform.Core](https://github.com/mosaabudosh/ArabazonPlatform.Core)
