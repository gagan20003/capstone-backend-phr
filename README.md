## Personal Health Record Management – Backend (ASP.NET Core, Oracle, JWT)

This repository contains the **backend API** for a Personal Health Record (PHR) system built on **ASP.NET Core 8**, **Entity Framework Core 9**, **Oracle Database**, and **JWT-based authentication**.  
It provides secure endpoints for managing **users, profiles, medical records, appointments, allergies, and medications**, along with role-based access and structured domain layers (Controllers → Services → Repositories → DbContext → Oracle).

---

## Table of Contents

- **Overview**
- **High-Level Architecture**
- **C4-style Diagrams**
  - System Context (C1)
  - Container Diagram (C2)
  - Component Diagram (C3 – backend focus)
  - Request/Response Flow
- **Domain Model**
  - Domain Model ER Diagram
- **Additional Architecture Diagrams**
  - Authentication Flow Diagram
  - Middleware Pipeline Diagram
  - Deployment Architecture Diagram
  - Data Flow Diagram
- **Project Structure**
- **Technology Stack**
- **Configuration**
- **Getting Started**
  - Prerequisites
  - Local setup
  - Database migrations
  - Running the API
- **API Surface (High-Level)**
- **Cross-Cutting Concerns**
  - Authentication & Authorization
  - Global Exception Handling
  - CORS
  - Health Checks
- **Testing**
- **Development & Coding Guidelines**
- **Troubleshooting**
- **Using Diagrams in Technical Documents**

---

## Overview

The **Personal Health Record Management** backend exposes RESTful APIs to:

- **Authenticate users** (register, login) using ASP.NET Identity + JWT.
- Manage **user profiles** (demographic and health-related data).
- Manage **medical records** (record type, provider, descriptions, dates, file URLs).
- Manage **appointments**, **allergies**, and **medications**.
- Enforce that every domain resource is **scoped to the logged-in user**.
- Persist data in an **Oracle database**, using Entity Framework Core with code-first migrations.

The backend is designed to be consumed by any frontend client (e.g., React, Angular, Vue, mobile apps) via HTTP/JSON and is documented with **Swagger/OpenAPI**.

---

## High-Level Architecture

The solution follows a typical layered architecture:

- **API Layer (Controllers)**  
  - ASP.NET Core MVC controllers, responsible for HTTP handling, validation, and mapping to services.

- **Application Layer (Services)**  
  - Business logic and orchestration, working with repositories and DTOs.

- **Data Access Layer (Repositories + DbContext)**  
  - Repository interfaces and their implementations using **EF Core**.
  - `AppDbContext` maps entities to tables and configures relationships and Oracle-specific settings.

- **Infrastructure / Cross-Cutting**  
  - ASP.NET Identity for authentication/authorization and roles.
  - JWT token generation and validation.
  - CORS, Logging, Global Exception Middleware, Health Checks.

---

## C4-style Diagrams

> **Note**: These diagrams are available in both **Mermaid code** (rendered automatically on GitHub/GitLab) and **image format** (PNG/SVG).  
> The images should be placed in a `diagrams/` folder at the root of the repository.  
> To export Mermaid diagrams as images, paste each code block into `https://mermaid.live` and download as PNG/SVG.

### System Context Diagram (C1)

![System Context Diagram](diagrams/system-context-diagram.png)

*Figure 1: System Context Diagram showing the relationship between users, frontend, backend API, and Oracle database.*

```mermaid
flowchart LR
    U[User] --> FE["PHR Frontend Application<br/>(Web / Mobile)"]
    FE -->|HTTPS / JSON| API["PHR Backend API<br/>(this project)"]
    API --> DB[("Oracle Database")]
    API --> JWT["JWT Token Infrastructure"]
```

**Key relationships**
- **User → Frontend**: User interacts via browser/mobile.
- **Frontend → Backend API**: Auth + CRUD on PHR resources.
- **Backend → Oracle DB**: Persistence for identity and health records.
- **Backend → JWT**: Issues and validates tokens for secured endpoints.

---

### Container Diagram (C2)

![Container Diagram](diagrams/container-diagram.png)

*Figure 2: Container Diagram showing the layered architecture of the backend API.*

```mermaid
flowchart TB
    FE["Frontend Client<br/>(SPA / Mobile)"] -->|HTTP / JSON| WEB["Web API Host<br/>(Kestrel / IIS Express)"]

    subgraph Backend["Backend API: .NET 8 ASP.NET Core"]
        WEB --> CTRL["API Layer<br/>Controllers"]
        CTRL --> SRV["Application Layer<br/>Domain Services"]
        SRV --> REPO["Data Access Layer<br/>Repositories"]
        REPO --> CTX["AppDbContext<br/>EF Core"]
        CTX --> ORA[("Oracle Database<br/>PHR Schema")]

        WEB --> ID["ASP.NET Identity & Auth"]
        ID --> CTX
    end
```

---

### Component Diagram (C3 – Backend Focus)

![Component Diagram](diagrams/component-diagram.png)

*Figure 3: Component Diagram showing the detailed structure of Controllers, Services, Repositories, and their relationships.*

```mermaid
flowchart TB

    subgraph Controllers
        A1[AuthController]
        A2[UserProfileController]
        A3[RecordsController]
        A4[AppointmentsController]
        A5[AllergiesController]
        A6[MedicationsController]
    end

    subgraph Services
        S0[JwtTokenService]
        S1[UserProfileService]
        S2[MedicalRecordService]
        S3[AppointmentService]
        S4[AllergyService]
        S5[MedicationService]
    end

    subgraph Repositories
        R1[IUserProfileRepository<br/>UserProfileRepository]
        R2[IMedicalRecordRepository<br/>MedicalRecordRepository]
        R3[IAppointmentRepository<br/>AppointmentRepository]
        R4[IAllergyRepository<br/>AllergyRepository]
        R5[IMedicationRepository<br/>MedicationRepository]
    end

    DBCTX[AppDbContext<br/>Oracle EF Core] --> DB[(Oracle Database)]

    %% Controller to Service
    A1 --> S0
    A2 --> S1
    A3 --> S2
    A4 --> S3
    A5 --> S4
    A6 --> S5

    %% Service to Repository
    S1 --> R1
    S2 --> R2
    S3 --> R3
    S4 --> R4
    S5 --> R5

    %% Repository to DbContext
    R1 --> DBCTX
    R2 --> DBCTX
    R3 --> DBCTX
    R4 --> DBCTX
    R5 --> DBCTX
```

---


## Domain Model (Key Entities & Relationships)

**Entities** (in `Models`):

- **`ApplicationUser`**  
  - Inherits from ASP.NET Identity user.  
  - Has one-to-one relationship with `UserProfile`.

- **`UserProfile`**  
  - One-to-one with `ApplicationUser` (`UserProfile.UserId` as FK).  
  - One-to-many relationships with `Allergies` and `Medications`.

- **`MedicalRecords`**  
  - Linked to `ApplicationUser` via `UserId`.  
  - Contains `RecordType`, `Provider`, `Description`, `RecordDate`, `FileUrl`, `CreatedAt`.

- **`Appointments`**  
  - Linked to `ApplicationUser` via `UserId`.  
  - Captures appointment date/time, provider, purpose, etc.

- **`Allergies` & `Medications`**  
  - Linked to `UserProfile` via `UserProfileId`.  
  - Store allergy and medication-related properties.

**Relationships** (configured in `AppDbContext`):

- `ApplicationUser` 1–1 `UserProfile` (unique `UserId` constraint).
- `ApplicationUser` 1–* `Appointments`.
- `ApplicationUser` 1–* `MedicalRecords`.
- `UserProfile` 1–* `Allergies`.
- `UserProfile` 1–* `Medications`.
- All bool properties are mapped to Oracle `NUMBER(1)` and **table/column names are uppercased**.

### Domain Model ER Diagram

![Domain Model ER Diagram](diagrams/domain-model-er-diagram.png)

*Figure 5: Entity-Relationship Diagram showing all database entities and their relationships.*

```mermaid
erDiagram
    APPLICATIONUSER ||--|| USERPROFILE : "has one"
    APPLICATIONUSER ||--o{ MEDICALRECORDS : "has many"
    APPLICATIONUSER ||--o{ APPOINTMENTS : "has many"
    USERPROFILE ||--o{ ALLERGIES : "has many"
    USERPROFILE ||--o{ MEDICATIONS : "has many"

    APPLICATIONUSER {
        string Id PK
        string UserName
        string Email
        string FullName
        string PasswordHash
    }

    USERPROFILE {
        int ProfileId PK
        string UserId FK
        string DateOfBirth
        string Gender
        string BloodType
        string PhoneNumber
        string Address
    }

    MEDICALRECORDS {
        int RecordId PK
        string UserId FK
        string RecordType
        string Provider
        string Description
        date RecordDate
        string FileUrl
        datetime CreatedAt
    }

    APPOINTMENTS {
        int AppointmentId PK
        string UserId FK
        datetime AppointmentDate
        string Provider
        string Purpose
        string Notes
    }

    ALLERGIES {
        int AllergyId PK
        int UserProfileId FK
        string AllergenName
        string Severity
        string Reaction
    }

    MEDICATIONS {
        int MedicationId PK
        int UserProfileId FK
        string MedicationName
        string Dosage
        string Frequency
        date StartDate
        date EndDate
    }
```

---

## Project Structure

```text
PersonalHealthRecordManagement.sln
└── PersonalHealthRecordManagement/
    ├── Program.cs                         # Composition root & middleware pipeline
    ├── appsettings.json                   # Base configuration (Oracle, JWT, CORS)
    ├── Controllers/
    │   ├── AuthController.cs              # Register / login
    │   ├── UserProfileController.cs
    │   ├── RecordsController.cs           # Medical records CRUD for current user
    │   ├── AppointmentsController.cs
    │   ├── AllergiesController.cs
    │   ├── MedicationsController.cs
    │   └── BaseController.cs              # Shared helpers (current user, responses)
    ├── Data/
    │   └── AppDbContext.cs                # EF Core DbContext, mapping & conventions
    ├── DTOs/                              # Request/response DTO definitions
    ├── Middleware/
    │   └── GlobalExceptionHandlerMiddleware.cs
    ├── Migrations/                        # EF Core migrations for Oracle
    ├── Models/                            # EF/Core entity classes
    ├── Repositories/                      # Repository interfaces + implementations
    ├── Services/                          # Business services + JwtTokenService
    ├── Tests/
    │   ├── AppointmentControllerTest.cs
    │   ├── AppointmentServiceTest.cs
    │   ├── RecordControllerTest.cs
    │   └── RecordServiceTest.cs
    └── PersonalHealthRecordManagement.csproj
```

---

## Technology Stack

- **Runtime & Framework**
  - .NET 8 (`net8.0`)
  - ASP.NET Core Web API

- **Authentication & Security**
  - ASP.NET Core Identity (`Microsoft.AspNetCore.Identity.EntityFrameworkCore`)
  - JWT Bearer Auth (`Microsoft.AspNetCore.Authentication.JwtBearer`)
  - Custom `JwtTokenService` for token creation.

- **Persistence**
  - Entity Framework Core 9 (`Microsoft.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.Tools`, `Microsoft.EntityFrameworkCore.Design`)
  - Oracle EF Core Provider (`Oracle.EntityFrameworkCore`)

- **API Tooling**
  - Swagger / Swashbuckle (`Swashbuckle.AspNetCore`)

- **Testing**
  - xUnit
  - Moq
  - FluentAssertions
  - `Microsoft.NET.Test.Sdk`

---

## Configuration

### `appsettings.json`

- **Logging**
  - Configures log levels for default and ASP.NET components.

- **CORS / Allowed Origins**
  - `AllowedOrigins`: array of allowed frontend origins (e.g. `http://localhost:3000`, `5173`, `4200`).

- **Database**
  - `ConnectionStrings:DefaultConnection` – Oracle connection string, e.g.:
    - `User Id=system;Password=1234;Data Source=//localhost:1521/xe;`

- **JWT**
  - `Jwt:Key` – symmetric signing key (keep secret in production!).
  - `Jwt:Issuer` – token issuer (`PHRAPI`).
  - `Jwt:Audience` – token audience (`PHRAPIUSERS`).
  - `Jwt:ExpiresMinutes` – token lifetime.

> **Important**: For production, never commit real secrets; use environment variables or secure configuration providers.

---

## Getting Started

### Prerequisites

- **.NET 8 SDK** installed.
- **Oracle Database** (e.g., XE or standard edition) running and accessible.
  - Listener configured (e.g. `localhost:1521/xe`).
  - A user/schema with required privileges or use `system` for dev only.
- **EF Core Tools** (optional but recommended for migrations):

```bash
dotnet tool install --global dotnet-ef
```

---

### 1. Clone the repository

```bash
git clone <your-repo-url>
cd capstone-backend-phr-feat-refactor
```

---

### 2. Configure the database & JWT

Open `PersonalHealthRecordManagement/appsettings.json` and update:

- **Connection string** (`ConnectionStrings:DefaultConnection`) with your own Oracle host, port, service name, username, and password.
- **Jwt:Key**, **Issuer**, **Audience** if needed.
- **AllowedOrigins** to match your frontend URLs.

You can also override via environment-specific configuration (e.g., `appsettings.Development.json`) or environment variables.

---

### 3. Restore dependencies

From the solution root:

```bash
cd PersonalHealthRecordManagement
dotnet restore
```

---

### 4. Apply database migrations

Migrations are located in the `Migrations` folder and are also executed at app startup via `db.Database.Migrate()` in `Program.cs`.  
Recommended process during development:

```bash
cd PersonalHealthRecordManagement

# Ensure the connection string for DefaultConnection is valid

# Apply migrations explicitly
dotnet ef database update
```

At runtime, the app will log:
- “Applying database migrations…”  
- “Database migrations applied successfully.”  
or log an error if migration fails.

---

### 5. Run the API

From `PersonalHealthRecordManagement/`:

```bash
dotnet run
```

The application will:

- Start the ASP.NET Core Kestrel server.
- Apply pending migrations (if any).
- Seed the **roles** `Admin` and `User` if they do not exist.
- Expose:
  - **Swagger UI** at `/swagger` (in Development).
  - **Health check** endpoint at `/health`.
  - API endpoints under `/api/...`.

---

## API Surface (High-Level)

> **Note**: Exact route names and payloads can be explored via Swagger UI or by inspecting controllers.

- **Authentication (`AuthController`)**
  - `POST /api/auth/register` – register a new user.
  - `POST /api/auth/login` – login, returns JWT and user details.

- **User Profile (`UserProfileController`)**
  - CRUD operations for the current user's profile.

- **Medical Records (`RecordsController`)**
  - `GET /api/records` – list medical records for current user.
  - `GET /api/records/{id}` – get a specific record (current user only).
  - `POST /api/records` – create a record for current user.
  - `PUT /api/records/{id}` – update a record for current user.
  - `DELETE /api/records/{id}` – delete a record for current user.

- **Appointments (`AppointmentsController`)**
  - CRUD endpoints for user-specific appointments.

- **Allergies (`AllergiesController`)**
  - CRUD endpoints for allergy data associated with the user’s profile.

- **Medications (`MedicationsController`)**
  - CRUD endpoints for user medications linked to the user’s profile.

- **Utility**
  - `GET /health` – simple health check for monitoring.
  - `WeatherForecastController` – sample/demo endpoint.

All resource controllers (records, appointments, allergies, medications, profile) use the **current user ID from the JWT** to ensure users can only access their own data.

---

## Additional Architecture Diagrams

### Authentication Flow Diagram

![Authentication Flow Diagram](diagrams/authentication-flow-diagram.png)

*Figure 6: Sequence diagram showing the registration and login authentication flows.*

```mermaid
sequenceDiagram
    participant User
    participant Frontend
    participant AuthController
    participant UserManager
    participant JwtService
    participant DB as Oracle DB

    Note over User,DB: Registration Flow
    User->>Frontend: Submit registration form
    Frontend->>AuthController: POST /api/auth/register
    AuthController->>UserManager: CreateAsync(user, password)
    UserManager->>DB: Insert user + hash password
    DB-->>UserManager: User created
    UserManager->>UserManager: AddToRoleAsync(user, "User")
    UserManager->>DB: Assign role
    AuthController-->>Frontend: 200 OK - User created
    Frontend-->>User: Registration success

    Note over User,DB: Login Flow
    User->>Frontend: Submit login credentials
    Frontend->>AuthController: POST /api/auth/login
    AuthController->>UserManager: FindByEmailAsync(email)
    UserManager->>DB: Query user
    DB-->>UserManager: User data
    AuthController->>UserManager: CheckPasswordAsync(user, password)
    UserManager->>DB: Verify password hash
    DB-->>UserManager: Password valid
    AuthController->>UserManager: GetRolesAsync(user)
    UserManager->>DB: Query roles
    DB-->>UserManager: User roles
    AuthController->>JwtService: CreateToken(user, roles)
    JwtService-->>AuthController: JWT token
    AuthController-->>Frontend: 200 OK + {token, userDetails}
    Frontend-->>User: Login success + store token
```

### Middleware Pipeline Diagram

![Middleware Pipeline Diagram](diagrams/middleware-pipeline-diagram.png)

*Figure 7: Flowchart showing the complete middleware pipeline and request processing flow.*

```mermaid
flowchart LR
    REQ[Incoming HTTP Request] --> HTTPS[HTTPS Redirection]
    HTTPS --> CORS[CORS Middleware]
    CORS --> AUTH[Authentication Middleware<br/>JWT Bearer]
    AUTH --> AUTHZ[Authorization Middleware]
    AUTHZ --> EXC[Global Exception Handler]
    EXC --> ROUTE[Routing Middleware]
    ROUTE --> CTRL[Controller Action]
    CTRL --> SRV[Service Layer]
    SRV --> REPO[Repository Layer]
    REPO --> DB[(Oracle Database)]
    DB --> REPO
    REPO --> SRV
    SRV --> CTRL
    CTRL --> RESP[HTTP Response]
    RESP --> EXC
    EXC --> CORS
    CORS --> HTTPS
    HTTPS --> CLIENT[Client Response]
```

### Deployment Architecture Diagram

![Deployment Architecture Diagram](diagrams/deployment-architecture-diagram.png)

*Figure 8: Deployment architecture showing client layer, network layer, server infrastructure, and external services.*

```mermaid
flowchart TB
    subgraph Client["Client Layer"]
        WEB[Web Browser]
       
    end

    subgraph Network["Network Layer"]
        LB[Load Balancer<br/>Optional]
        FW[Firewall]
    end

    subgraph Server["Server Infrastructure"]
        subgraph AppServer["Application Server"]
            API1[ASP.NET Core API<br/>Instance 1]
            API2[ASP.NET Core API<br/>Instance 2<br/>Optional]
        end
        
        subgraph DBServer["Database Server"]
            ORA[(Oracle Database<br/>Primary)]
            ORA_BACKUP[(Oracle Database<br/>Backup/Replica<br/>Optional)]
        end
    end

    subgraph External["External Services"]
        SWAGGER[Swagger UI<br/>API Documentation]
        HEALTH[Health Check<br/>Monitoring]
    end

    WEB --> LB
    MOB --> LB
    LB --> FW
    FW --> API1
    FW --> API2
    API1 --> ORA
    API2 --> ORA
    ORA -.->|Replication| ORA_BACKUP
    API1 --> SWAGGER
    API1 --> HEALTH
    API2 --> SWAGGER
    API2 --> HEALTH
```

### Data Flow Diagram (CRUD Operation Example)

![Data Flow Diagram](diagrams/data-flow-diagram.png)

*Figure 9: Flowchart showing the complete data flow for CRUD operations including authentication, validation, business rules, and error handling.*

```mermaid
flowchart TD
    START([User Action]) --> REQ{Request Type}
    
    REQ -->|GET| READ[Read Operation]
    REQ -->|POST| CREATE[Create Operation]
    REQ -->|PUT| UPDATE[Update Operation]
    REQ -->|DELETE| DELETE[Delete Operation]
    
    READ --> AUTH_CHECK{Authenticated?}
    CREATE --> AUTH_CHECK
    UPDATE --> AUTH_CHECK
    DELETE --> AUTH_CHECK
    
    AUTH_CHECK -->|No| UNAUTH[401 Unauthorized]
    AUTH_CHECK -->|Yes| VALIDATE{Validate Input}
    
    VALIDATE -->|Invalid| BAD_REQ[400 Bad Request]
    VALIDATE -->|Valid| SERVICE[Service Layer]
    
    SERVICE --> BUSINESS{Business Rules}
    BUSINESS -->|Fail| ERROR[Error Response]
    BUSINESS -->|Pass| REPO[Repository Layer]
    
    REPO --> DB_OP[(Database Operation)]
    DB_OP -->|Success| MAP[Map to DTO]
    DB_OP -->|Fail| DB_ERROR[Database Error]
    
    MAP --> RESPONSE[200 OK Response]
    ERROR --> RESPONSE
    DB_ERROR --> ERROR
    BAD_REQ --> RESPONSE
    UNAUTH --> RESPONSE
    
    RESPONSE --> END([Return to Client])
```

---

- **Identity configuration** (in `Program.cs`):
  - Password rules (min length, digits, uppercase/lowercase).
  - User and role stores via `AppDbContext`.
- **JWT configuration**:
  - Default authentication scheme: `JwtBearerDefaults.AuthenticationScheme`.
  - Validates **Issuer**, **Audience**, **Signing Key**, and **Lifetime**.
- **Roles**:
  - `Admin`, `User` created at startup if missing.
  - Controllers can use `[Authorize]` and role policies (extend as needed).

### Global Exception Handling

- `GlobalExceptionHandlerMiddleware` is added into the pipeline:
  - Catches unhandled exceptions.
  - Logs errors.
  - Returns consistent error responses.

### CORS

- Configured using `builder.Services.AddCors` with `AllowedOrigins` from configuration.
- Policy:
  - Allows any method and header.
  - Allows credentials.

### Health Checks

- `builder.Services.AddHealthChecks()` and `app.MapHealthChecks("/health")`.
- Designed for use with load balancers, Kubernetes, or monitoring systems.

---

## Testing

Tests are located under `PersonalHealthRecordManagement/Tests`:

- **Unit/Integration tests (examples):**
  - `AppointmentControllerTest.cs`
  - `AppointmentServiceTest.cs`
  - `RecordControllerTest.cs`
  - `RecordServiceTest.cs`

**Test stack:**

- **xUnit** – test framework.
- **Moq** – mocking dependencies (e.g., services, repositories).
- **FluentAssertions** – expressive assertions.

Run tests from the `PersonalHealthRecordManagement` project directory:

```bash
dotnet test
```

---

## Development & Coding Guidelines

- **Layered separation**:
  - Keep controllers lean (HTTP + model binding, minimal business logic).
  - Put business rules into `Services`.
  - Keep persistence logic in `Repositories` and `AppDbContext`.

- **DTO usage**:
  - Use DTOs (from `DTOs` folder) for request and response payloads.
  - Avoid exposing EF entities directly to external clients when possible.

- **Error handling**:
  - Prefer using the global exception middleware for unhandled cases.
  - Return meaningful error messages and proper HTTP status codes.

- **Security**:
  - Protect sensitive configuration values.
  - Validate model state for all input DTOs.
  - Restrict access via `[Authorize]` or roles when exposing admin-specific endpoints.

---

## Troubleshooting

- **App fails to connect to Oracle**
  - Check `DefaultConnection` string (host, port, service name, credentials).
  - Ensure Oracle listener is up and reachable from the machine running the API.
  - Confirm that the Oracle EF Core provider version matches your Oracle DB version.

- **Migrations fail on startup**
  - Try running `dotnet ef database update` manually to see detailed errors.
  - Confirm the user has permissions to create/alter tables.
  - Review the logs emitted during app startup (`Applying database migrations...`).

- **CORS errors from frontend**
  - Ensure the frontend URL is present in `AllowedOrigins`.
  - Verify protocol/port (e.g., `http://localhost:5173` vs `https://...`).

- **JWT / 401 Unauthorized**
  - Ensure the `Authorization: Bearer <token>` header is included.
  - Confirm the token is not expired and the **Audience/Issuer** match server configuration.
  - If keys changed, old tokens will no longer work; log in again to obtain a fresh token.

---

## Extending the System

- **New domain module** (e.g., lab results, imaging records):
  - Add entity to `Models`.
  - Configure relationships in `AppDbContext`.
  - Create repository interface + implementation.
  - Create service interface + implementation.
  - Add controller endpoints and corresponding DTOs.
  - Add tests for service and controller.

- **Additional auth features**:
  - Add role-based policies or custom authorization handlers.
  - Implement refresh tokens or multi-factor auth as needed.

---


