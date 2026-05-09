# Student Management System

Production-ready full-stack Student Management application built with **ASP.NET Core 9** (clean layered architecture) and **Angular 21**, secured with **JWT**.

> The user requested .NET 8, but only .NET 6, 9, and 10 SDKs were installed on this machine. The project targets **.NET 9** which is API-compatible and uses the same patterns. Change `<TargetFramework>` to `net8.0` in every `.csproj` if you install the .NET 8 SDK.

---

## Table of Contents
- [Project Overview](#project-overview)
- [Technologies](#technologies)
- [Solution Architecture](#solution-architecture)
- [Folder Structure](#folder-structure)
- [Prerequisites](#prerequisites)
- [Configuration](#configuration)
- [Database Setup](#database-setup)
- [Run the Backend](#run-the-backend)
- [Run the Frontend](#run-the-frontend)
- [Default Credentials](#default-credentials)
- [API Endpoints](#api-endpoints)
- [Testing the API](#testing-the-api)
- [Standardised API Response](#standardised-api-response)
- [Best Practices Highlights](#best-practices-highlights)

---

## Project Overview

A small but realistic Student Management System featuring:

- **JWT authentication** with Bearer-token-protected endpoints.
- **Student CRUD** with pagination, search, sorting, and unique-email validation.
- **Clean Architecture** (Domain → Application → Infrastructure → API).
- **Repository + Unit of Work** pattern over Entity Framework Core 9.
- **AutoMapper** for entity ↔ DTO conversion.
- **FluentValidation** for input validation.
- **Serilog** structured logging to console and rolling files.
- **Global Exception Middleware** producing a uniform `ApiResponse` envelope.
- **Swagger / OpenAPI** with JWT authorisation in the UI.
- **Angular 21** standalone-component frontend with Reactive Forms, Route Guards, JWT interceptor, error interceptor, loader interceptor, Toast notifications, search, sorting and pagination.

## Technologies

**Backend**
- ASP.NET Core 9 Web API
- Entity Framework Core 9 (SQL Server)
- AutoMapper, FluentValidation
- Serilog (Console + File sinks)
- Swashbuckle (Swagger / OpenAPI)
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- PBKDF2 password hashing (`Microsoft.AspNetCore.Cryptography.KeyDerivation`)

**Frontend**
- Angular 21 (standalone components, signals, zoneless change detection)
- Bootstrap 5 + Bootstrap Icons
- Reactive Forms
- ngx-toastr
- jwt-decode

## Solution Architecture

```
┌───────────────────────────────────────────────┐
│                   API (Web)                    │  ← Controllers, Middleware, Filters,
│  Program.cs · Controllers · Middleware ·       │    Auth wiring, Swagger, CORS
│  Filters · Extensions · appsettings.json       │
├───────────────────────────────────────────────┤
│                 Application                    │  ← Use cases, DTOs, Interfaces,
│  Services · DTOs · Interfaces · Validators ·   │    AutoMapper Profiles, Validators,
│  Mappings · Common (Responses/Helpers/Excs.)   │    ApiResponse contract
├───────────────────────────────────────────────┤
│                  Infrastructure                │  ← EF Core, Repositories,
│  Persistence (DbContext, Configurations,       │    JWT issuance, password hashing,
│  Repositories, Migrations, Seeder) ·           │    seeding
│  Authentication (PasswordHasher, JwtTokenSvc)  │
├───────────────────────────────────────────────┤
│                     Domain                     │  ← Entities + Enums (no deps)
│  Entities (Student, ApplicationUser) · Enums   │
└───────────────────────────────────────────────┘

Reference flow: API → Application + Infrastructure
                Infrastructure → Application → Domain
                Domain has no project references.
```

## Folder Structure

```
CrudWithJwt/
├─ backend/
│  ├─ StudentManagement.sln
│  ├─ src/
│  │  ├─ StudentManagement.API/
│  │  │  ├─ Controllers/             (AuthController, StudentsController)
│  │  │  ├─ Extensions/              (Auth, Cors, Swagger DI extensions)
│  │  │  ├─ Filters/                 (ValidationActionFilter)
│  │  │  ├─ Middleware/              (GlobalExceptionMiddleware)
│  │  │  ├─ Properties/launchSettings.json
│  │  │  ├─ appsettings.json / appsettings.Development.json
│  │  │  └─ Program.cs               (composition root)
│  │  ├─ StudentManagement.Application/
│  │  │  ├─ Common/
│  │  │  │  ├─ Exceptions/           (NotFound, Conflict)
│  │  │  │  ├─ Helpers/              (EnumExtensions)
│  │  │  │  └─ Responses/            (ApiResponse, PagedResult)
│  │  │  ├─ DTOs/                    (Auth, Student)
│  │  │  ├─ Interfaces/              (Authentication, Persistence,
│  │  │  │                            Repositories, Services)
│  │  │  ├─ Mappings/                (StudentProfile)
│  │  │  ├─ Services/                (AuthService, StudentService)
│  │  │  ├─ Validators/              (FluentValidation rules)
│  │  │  └─ DependencyInjection.cs
│  │  ├─ StudentManagement.Domain/
│  │  │  ├─ Common/AuditableEntity.cs
│  │  │  ├─ Entities/                (Student, ApplicationUser)
│  │  │  └─ Enums/                   (UserRole, ResponseMessage)
│  │  └─ StudentManagement.Infrastructure/
│  │     ├─ Authentication/          (JwtTokenService, PasswordHasher)
│  │     ├─ Configurations/          (JwtSettings)
│  │     ├─ Persistence/
│  │     │  ├─ Configurations/       (Fluent API entity configs)
│  │     │  ├─ Migrations/           (EF Core migrations)
│  │     │  ├─ Repositories/
│  │     │  ├─ Seed/                 (DatabaseSeeder)
│  │     │  └─ AppDbContext.cs
│  │     └─ DependencyInjection.cs
│  └─ scripts/
│     └─ InitialSchema.sql           (idempotent migration script)
│
├─ frontend/
│  ├─ angular.json · package.json · proxy.conf.json
│  └─ src/
│     ├─ environments/
│     ├─ index.html · main.ts · styles.scss
│     └─ app/
│        ├─ app.ts · app.config.ts · app.routes.ts
│        ├─ core/
│        │  ├─ guards/               (authGuard, guestGuard)
│        │  ├─ interceptors/         (auth, error, loader)
│        │  ├─ models/               (api-response, auth, student)
│        │  └─ services/             (AuthService, StudentService, LoaderService)
│        ├─ features/
│        │  ├─ auth/login/
│        │  ├─ dashboard/
│        │  └─ students/
│        │     ├─ student-list/
│        │     └─ student-form/
│        └─ shared/
│           ├─ components/loader/
│           └─ layout/shell/
│
├─ README.md
└─ .gitignore
```

## Prerequisites

| Tool | Version |
| --- | --- |
| .NET SDK | 9.0+ (or 8.0 — change `TargetFramework` if needed) |
| SQL Server | 2019+ (Express or Developer is fine) |
| Node.js | 20+ |
| Angular CLI | 21+ (the project pins `@angular/cli` and `@angular/build` already) |

Install the EF Core tools globally if not already present:

```bash
dotnet tool install --global dotnet-ef --version 9.0.0
```

## Configuration

All sensitive configuration lives in `backend/src/StudentManagement.API/appsettings.json`:

```jsonc
{
  "ConnectionStrings": {
    "Default": "Server= Vishal; Database=Crud;Trusted_Connection=true;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "w+rQUjqIMTL1+h5tZqQUMxQ4VS0s/9mfp+ui/wd1fWGVcC0qjh+X+SrbP0m/Cl7OzwGfD6agIsx4ZRzNiXJuLw==",
    "Issuer": "StudentManagement.API",
    "Audience": "StudentManagement.Client",
    "AccessTokenExpiryMinutes": 60
  },
  "Cors": {
    "AllowedOrigins": [ "http://localhost:4200" ]
  }
}
```

> **Production tip**: never commit secrets. Use `dotnet user-secrets`, environment variables, Azure Key Vault, or AWS Secrets Manager in real deployments.

## Database Setup

Three options — pick one.

**Option A — apply existing migration (recommended)**

```bash
cd backend
dotnet ef database update \
  --project src/StudentManagement.Infrastructure \
  --startup-project src/StudentManagement.API
```

**Option B — let the API auto-migrate at startup**

The seeder in `Program.cs` calls `Database.MigrateAsync()` automatically the first time the API starts.

**Option C — run the SQL script manually**

```bash
sqlcmd -S Vishal -d Crud -i backend/scripts/InitialSchema.sql
```

The migration creates the `Users` and `Students` tables with appropriate indexes and constraints. On first run the seeder also inserts the default admin and 5 sample students.

To regenerate the SQL script after a schema change:

```bash
cd backend
dotnet ef migrations script \
  --project src/StudentManagement.Infrastructure \
  --startup-project src/StudentManagement.API \
  --output scripts/InitialSchema.sql --idempotent
```

## Run the Backend

```bash
cd backend
dotnet run --project src/StudentManagement.API
```

Default Kestrel URLs (see `Properties/launchSettings.json`):

| Endpoint | URL |
| --- | --- |
| HTTPS | `https://localhost:7095` |
| HTTP | `http://localhost:5095` |
| Swagger | `https://localhost:7095/swagger` |
| Health | `https://localhost:7095/health` |

If those ports are taken, update `launchSettings.json` and `frontend/proxy.conf.json` accordingly.

## Run the Frontend

```bash
cd frontend
npm install   # only the first time
npm start     # ng serve with proxy.conf.json
```

The dev server proxies `/api` and `/health` to the backend so cookies, CORS and HTTPS just work. Open `http://localhost:4200` in your browser.

## Default Credentials

| Field | Value |
| --- | --- |
| Username | `admin` |
| Password | `Admin@123` |

These are seeded automatically by `DatabaseSeeder` the first time the API starts (only when the `Users` table is empty). Change the password in production by hashing your own value via `PasswordHasher` and updating the row.

## API Endpoints

All student endpoints require an `Authorization: Bearer <token>` header.

| Method | Route | Body / Query | Description |
| --- | --- | --- | --- |
| POST | `/api/auth/login` | `{ username, password }` | Returns a signed JWT |
| GET  | `/api/students` | `pageNumber`, `pageSize`, `search`, `course`, `sortBy`, `sortDescending` | Paged, searchable, sortable list |
| GET  | `/api/students/{id}` | — | Get single student |
| POST | `/api/students` | `{ name, email, age, course }` | Create student |
| PUT  | `/api/students/{id}` | `{ id, name, email, age, course, isActive }` | Update student |
| DELETE | `/api/students/{id}` | — | Delete student |
| GET  | `/health` | — | Health probe |

`sortBy` accepts: `name`, `email`, `age`, `course`, `createddate`.

## Testing the API

**Swagger** — easiest

1. Run the backend.
2. Open `https://localhost:7095/swagger`.
3. `POST /api/auth/login` with the default credentials.
4. Copy `data.accessToken` from the response.
5. Click the **Authorize** button (padlock) and paste the token. Bearer prefix is added automatically.
6. Use any of the student endpoints.

**curl**

```bash
# Login
curl -k -X POST https://localhost:7095/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}'

# List students
curl -k https://localhost:7095/api/students?pageNumber=1&pageSize=10 \
  -H "Authorization: Bearer <PASTE_TOKEN_HERE>"
```

## Standardised API Response

Every success looks like this:

```json
{
  "success": true,
  "message": "Student created successfully.",
  "data": { "id": 12, "name": "Aarav Sharma", "email": "...", "...": "..." },
  "errors": null,
  "statusCode": 201
}
```

Every failure follows the same shape:

```json
{
  "success": false,
  "message": "Validation failed. Please verify the highlighted fields.",
  "data": null,
  "errors": [ "Email format is invalid." ],
  "statusCode": 400
}
```

`GlobalExceptionMiddleware` maps known exception types to HTTP statuses:

| Exception | Status |
| --- | --- |
| `ValidationException` | 400 |
| `NotFoundException` | 404 |
| `ConflictException` | 409 |
| `UnauthorizedAccessException` | 401 |
| `SqlException` | 500 (with sanitised message) |
| Anything else | 500 |

## Best Practices Highlights

- **No magic strings** — every user-facing message routes through the `ResponseMessage` enum and `EnumExtensions.GetDescription()`.
- **DTOs only over the wire** — entities never leave the `Infrastructure` boundary.
- **Repository + UoW** — services depend on `IStudentRepository` / `IUnitOfWork`, never on `DbContext` directly.
- **`AsNoTracking`** for read paths in `StudentRepository`.
- **PBKDF2 hashing** with HMAC-SHA256, 100k iterations, 16-byte salt, time-constant verify (`CryptographicOperations.FixedTimeEquals`).
- **JWT** signed with HS256, configurable issuer/audience/lifetime; the symmetric key is read from configuration and normalised to a safe length at runtime.
- **Functional Angular interceptors** for auth header injection, global error toasts, and a request-counted spinner.
- **Standalone components + signals + lazy routes** — small bundles per feature.
- **CORS** locked to the configured Angular origin.
- **Health endpoint** returns the same `ApiResponse` envelope for monitoring tools.

---

## Useful Commands

```bash
# Backend
cd backend
dotnet build
dotnet run --project src/StudentManagement.API

# EF Core
dotnet ef migrations add <Name> \
  --project src/StudentManagement.Infrastructure \
  --startup-project src/StudentManagement.API \
  --output-dir Persistence/Migrations
dotnet ef database update \
  --project src/StudentManagement.Infrastructure \
  --startup-project src/StudentManagement.API

# Frontend
cd frontend
npm install
npm start                    # http://localhost:4200
npm run build                # production build to dist/
```

Happy shipping.
