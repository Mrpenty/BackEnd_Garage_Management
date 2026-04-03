# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ASP.NET Core 8.0 Web API for a garage/vehicle service management system (MSMG). C# solution with layered architecture using Entity Framework Core 8 + SQL Server.

## Common Commands

```bash
# Build
dotnet build Garage_Management.sln

# Run (http://localhost:5034, Swagger at /swagger)
dotnet run --project Garage_Management.API

# Test
dotnet test Garage_Management.UnitTest
dotnet test Garage_Management.UnitTest --filter "FullyQualifiedName~ClassName.MethodName"

# EF Core Migrations
dotnet ef migrations add MigrationName --project Garage_Management.Base --startup-project Garage_Management.API
dotnet ef database update --project Garage_Management.Base --startup-project Garage_Management.API

# Docker
docker build -t garage-management-api:latest .
```

## Architecture

Four-layer solution:

- **Garage_Management.API** — Controllers, middleware, DI extensions (`Extensions/`), `Program.cs` startup. Global exception handling via `ExceptionHandlingMiddleware`.
- **Garage_Management.Application** — Services (`Services/`), repository implementations (`Repositories/`), interfaces (`Interfaces/`), DTOs (`DTOs/`), validators (`Validators/`). DI auto-registration via Scrutor in `ApplicationDependencyInjection.cs`.
- **Garage_Management.Base** — Domain entities (`Entities/`), EF Core DbContext (`Data/AppDbContext.cs`), Fluent API configurations (`Data/Configurations/`), migrations, enums (`Common/Enums/`), base classes. `AuditableEntity` is the base class with CreatedAt/UpdatedAt/DeletedAt fields.
- **Garage_Management.UnitTest** — MSTest + Moq + EF InMemory tests.

## Key Patterns

- **Repository + Service pattern**: `IBaseRepository<T>` → `BaseRepository<T>` for generic CRUD; domain-specific repos and services layered on top.
- **Standardized responses**: All API responses use `ApiResponse<T>` wrapper.
- **Auth**: ASP.NET Identity with JWT Bearer tokens + Twilio OTP for customers. Token generation in `GenerateToken.cs` with role-based claims (CustomerID/EmployeeID).
- **Service registration**: Services auto-scanned via Scrutor; repositories registered manually in `InfrastructureDependencyInjection.cs`.

## Domain Areas

Entities are organized under `Garage_Management.Base/Entities/` by domain: Accounts, Vehicles (note: folder spelled `Vehiclies`), JobCards, Inventories, Services, RepairEstimaties (note spelling), Warranties.

## Database

`AppDbContext` extends `IdentityDbContext<User, IdentityRole<int>, int>` with 26+ DbSets. SQL Server with connection string in `appsettings.json`. Entity configurations use Fluent API in `Data/Configurations/`.
