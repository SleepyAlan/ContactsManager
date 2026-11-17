# ContactsManager

A production-style **ASP.NET Core (.NET 10)** contacts management application built with a layered architecture, authentication/authorization, robust validation, logging, and automated testing.

This project demonstrates a practical full-stack CRUD website.

## Project Summary

ContactsManager is a web app for managing people and countries, with support for:

- Contact CRUD (create, read, update, delete)
- Search and sorting across multiple fields
- Exporting contacts to **CSV**, **Excel**, and **PDF**
- Uploading countries from Excel
- User registration/login/logout with ASP.NET Core Identity
- Structured request and error logging with Serilog
- Unit and integration test coverage

---

## Tech Stack

### Platform
- .NET 10
- ASP.NET Core (Controllers + Views)
- Entity Framework Core 10 (SQL Server)

### Authentication & Security
- ASP.NET Core Identity
- Cookie authentication
- Fallback authorization policy (authenticated users required by default)
- Anti-forgery validation on form posts

### Data & Infrastructure
- SQL Server / LocalDB
- EF Core Migrations
- Seed data from JSON (`SeedData/Countries.json`, `SeedData/Persons.json`)
- Repository + Service abstractions

### Logging & Diagnostics
- Serilog
- Console sink
- Rolling file sink (`ContactsManager/Logs`)
- SQL Server sink
- Seq sink (optional)
- Request timing and diagnostic context usage

### File/Document Features
- CSV export via CsvHelper
- Excel export/import via EPPlus
- PDF export via Rotativa (wkhtmltopdf)

### Testing
- xUnit
- Moq
- AutoFixture
- ASP.NET Core `WebApplicationFactory` for integration tests
- InMemory EF Core database for integration test environment
- AngleSharp for HTML/DOM assertions

---

## Solution Structure

- `ContactsManager` - Web application (controllers, views, middleware, filters, startup)
- `Entities` - EF Core DbContext, entities, identity models, migrations
- `ServiceContracts` - DTOs, enums, and service interfaces
- `Services` - Business logic implementation
- `RepositoryContracts` - Repository abstractions
- `Repositories` - Data access implementation with EF Core
- `ContactsManagerTests` - Service-layer unit tests
- `ContactsManager.Tests` - Controller + integration tests

---

## Key Features Implemented

- Layered architecture with clear dependency boundaries
- Validation and exception handling in service layer
- Search filtering by name, email, date of birth, gender, country, address, newsletter, and age comparisons
- Sorting support by multiple contact fields and direction
- Partial rendering support for table refresh scenarios
- Data export endpoints:
  - `/Persons/PersonsCSV`
  - `/Persons/PersonsExcel`
  - `/Persons/PersonsPDF`
- Upload countries from Excel (`/Countries/UploadCountriesFromExcel`)
- Custom action filters:
  - Whitespace sanitization for request models
  - Request execution timing for person list
  - Redirect authenticated users away from auth pages
- Custom exception middleware + default exception handler integration

---

## Data Ownership Note

Identity is implemented for authentication/authorization, but contacts and countries are **not currently tied to user-specific tables/ownership columns**. This means all authenticated users interact with the same shared `Persons` and `Countries` data.

This is intentional for simpler demo/reviewer walkthroughs.

### How to make data per-account (concise plan)

1. Add `UserId` (`Guid`) and navigational properties to `Person` and `Country` entities.
2. Add FK relationships to `ApplicationUser` in `ApplicationDbContext`.
3. Create and apply an EF migration (`dotnet ef migrations add AddOwnershipToPersonCountry ...`, then `dotnet ef database update ...`).
4. In create/update/read/delete flows, use the logged-in user id (`UserManager.GetUserId(User)`) and filter queries by `OwnerUserId`.
5. Enforce ownership in services/repositories (reject access to records owned by other users).
6. Update seed/test data to include ownership values.

---

## Local Setup (After Pulling the Repository)

## 1) Prerequisites

Install:

- .NET 10 SDK
- SQL Server LocalDB (or SQL Server instance)
- Visual Studio 2026+ (recommended)

## 2) Clone

```powershell
git clone https://github.com/SleepyAlan/ContactsManager.git
cd ContactsManager
```

## 3) Restore dependencies

```powershell
dotnet restore
```

## 4) Configure connection string (if needed)

Default connection strings are in:

- `ContactsManager/appsettings.json`

By default, the app uses LocalDB. If you use another SQL Server instance, update:

- `ConnectionStrings:DefaultConnection`
- (Optional) Serilog MSSqlServer sink connection string

## 5) Apply EF Core migrations

```powershell
dotnet ef database update --project Entities/Entities.csproj --startup-project ContactsManager/ContactsManager.csproj
```

## 6) Run the web app

```powershell
dotnet run --project ContactsManager/ContactsManager.csproj
```

Default launch profile URLs:

- `https://localhost:7065`
- `http://localhost:5183`

---

## Test Execution

Run all tests via the IDE (recommended) or via the terminal:

```powershell
dotnet test
```

You can also run test projects independently:

```powershell
dotnet test ContactsManagerTests/Services.Tests.csproj
dotnet test ContactsManager.Tests/ContactsManager.Tests.csproj
```

---

## Notes

- `ContactsManager/wwwroot/Rotativa/wkhtmltopdf.exe` is included for PDF generation support.
- Seq sink is configured to `http://localhost:5341`; start a local Seq instance if you want centralized log viewing.

---

## Potential Next Improvements

- Add per-user data ownership for contacts and countries.
- Add unit tests for the repository layer.
- Utilize TestContainers with Docker to test against a real database.
- Add update/delete CRUD UI/action-method for countries.
