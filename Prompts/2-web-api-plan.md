**Dependencies**

- API NuGet: MySqlConnector, Swashbuckle.AspNetCore
- Test NuGet: xunit, xunit.runner.visualstudio, Microsoft.NET.Test.Sdk
- Framework: .NET 9 (`net9.0`), ASP.NET Core Minimal APIs (no extra packages)

**Data Model**

- Application: Id:int PK AUTO_INCREMENT; Name:varchar(100) not null; Description:varchar(500) null; CreatedUtc:datetime(6) not null; UpdatedUtc:datetime(6) not null
- Configuration: Id:int PK AUTO_INCREMENT; ApplicationId:int not null FK → Applications(Id) ON DELETE CASCADE; JsonContent:JSON not null; CreatedUtc:datetime(6) not null; UpdatedUtc:datetime(6) not null
- One-to-one: UNIQUE(ApplicationId) on `configurations` to enforce exactly one configuration per application

**Endpoints**

- Applications
  - GET `/applications/{id:int}`: 200 with application; 404 if not found
  - GET `/applications`: 200 list (no paging)
  - POST `/applications`: 201 with location; 400 on invalid payload
  - PUT `/applications/{id:int}`: 204 on success; 404 if not found; 400 on invalid payload
- Configurations
  - GET `/configurations/{id:int}`: 200 with configuration; 404 if not found
  - GET `/applications/{applicationId:int}/configuration`: 200 with configuration; 404 if not found
  - GET `/configurations`: 200 list (no paging)
  - POST `/configurations`: 201 with location; 400 on invalid payload or missing/invalid `ApplicationId`; 404 if `ApplicationId` refers to a non-existent application; 409 if a configuration already exists for the application
  - PUT `/configurations/{id:int}`: 204 on success; 404 if not found; 400 on invalid payload
- Payloads
  - Application create/update: { Name, Description? }
  - Configuration create/update: { ApplicationId (create only), JsonContent } where `JsonContent` is any JSON object/array; validated and stored in MySQL `JSON` column

**Architectural Patterns**

- Vertical slices: split by feature (Applications, Configurations) for endpoints and repositories
- Minimal APIs: endpoint files contain routing, validation, and orchestration (no separate Services layer)
- Repository pattern (raw ADO.NET via MySqlConnector): `IApplicationRepository`, `IConfigurationRepository`
- Connection factory: `IDbConnectionFactory` to create `MySqlConnection` (per-request scope)
- Error handling: consistent ProblemDetails-style responses (400/404/409); last-write-wins; use `CreatedUtc/UpdatedUtc` timestamps

**File/Folder Structure**

- Solution
  - `Configurator.sln`
  - `API/Program.cs`
  - `API/appsettings.json`
  - `API/Endpoints/ApplicationsEndpoints.cs`
  - `API/Endpoints/ConfigurationsEndpoints.cs`
  - `API/Domain/Application.cs`
  - `API/Domain/Configuration.cs`
  - `API/Data/IApplicationRepository.cs`
  - `API/Data/IConfigurationRepository.cs`
  - `API/Data/ApplicationRepository.cs`
  - `API/Data/ConfigurationRepository.cs`
  - `API/Infrastructure/IDbConnectionFactory.cs`
  - `API/Infrastructure/MySqlConnectionFactory.cs`
  - `database/sql/001_init.sql`
  - `tests/API.Tests/Endpoints/ApplicationsEndpoints.Tests.cs`
  - `tests/API.Tests/Endpoints/ConfigurationsEndpoints.Tests.cs`
  - `tests/API.Tests/Data/Fakes/FakeApplicationRepository.cs`
  - `tests/API.Tests/Data/Fakes/FakeConfigurationRepository.cs`

**Validation & Rules**

- Application: Name required (1–100 chars), Description ≤ 500 chars
- Configuration: `JsonContent` must be valid JSON (parse via System.Text.Json)
- Create Configuration: verify Application exists; enforce one configuration per application (repo check + DB UNIQUE constraint)
 - Update: full replace via PUT; application sets `UpdatedUtc = DateTime.UtcNow`

**SQL Migrations**

- `database/sql/001_init.sql`
  - Create schema (utf8mb4)
  - Tables:
    - `applications` with PK and DATETIME(6) `CreatedUtc`/`UpdatedUtc` (no DB defaults; set by application using UTC)
    - `configurations` with PK, FK to applications, UNIQUE(ApplicationId), JSON column, DATETIME(6) timestamps (no DB defaults; set by application using UTC)
  - Seed:
    - Insert one application; capture `LAST_INSERT_ID()` to variable
    - Insert one configuration for that application with a minimal JSON payload (e.g., JSON_OBJECT('featureFlag', true))

**Swagger/OpenAPI**

- Register `AddEndpointsApiExplorer()` and `AddSwaggerGen()`; enable Swagger UI in Development
- Annotate endpoints with minimal metadata (summary, response types) to improve the spec

**Raw ADO.NET Access**

- Always parameterize queries; map columns explicitly
- Use async methods (`OpenAsync`, `ExecuteNonQueryAsync`, `ExecuteReaderAsync`)
- Keep connections short-lived via `await using` and rely on MySqlConnector pooling
- Timeouts: set reasonable command timeout (e.g., 15–30s) via `MySqlCommand.CommandTimeout`

 

**Unit Testing Plan (xUnit, mirrored folders in test project)**

- Endpoints (primary coverage)
  - Applications: create/update validations (name required, description length), not-found on update, happy-path create/update/get/get-all.
  - Configurations: JSON validation, 400 vs 404 rules on ApplicationId, uniqueness rule (409), happy-path create/update/get-by-id/get-by-application-id/get-all.
- Test setup: use in-memory fake repositories (`FakeApplicationRepository`, `FakeConfigurationRepository`) injected into endpoints to simulate success and failure paths; no external packages.
- Structure: test files mirror the target files’ relative folder paths within the test project (e.g., `tests/.../Endpoints/ApplicationsEndpoints.Tests.cs` for `src/.../Endpoints/ApplicationsEndpoints.cs`).
- No DB/integration tests or mocking libraries (keeps within approved dependencies).

**Implementation Steps**

- Scaffold solution/projects (`src` and `tests`), set TFM `net9.0`.
- Add NuGet dependencies (API: MySqlConnector, Swashbuckle.AspNetCore; Tests: xunit, xunit.runner.visualstudio, Microsoft.NET.Test.Sdk).
- Implement Domain models (`Application`, `Configuration`).
- Implement `IDbConnectionFactory` and MySqlConnector-backed factory (reads `ConnectionStrings:Default`).
- Implement repositories (raw SQL, parameterized) for Applications/Configurations.
- Implement Minimal API endpoint files with inline request DTOs and validation/orchestration (JSON parse, existence/uniqueness checks).
- Wire DI for connection factory and repositories; add Swagger.
- Write SQL migration `001_init.sql` (schema + seed).
- Add unit tests with in-memory fakes; cover main scenarios.
- Verify locally: run SQL script, set connection string, start API, run tests.

**Configuration & Run**

- Connection string: `ConnectionStrings:Default` (e.g., `Server=localhost;Port=3306;Database=hypr_config;User ID=...;Password=...;SslMode=None;`)
- Environment: `ASPNETCORE_ENVIRONMENT=Development` to enable Swagger
- Apply schema: `mysql -u <user> -p <password> < hypr_config < database/sql/001_init.sql`
- Run API: `dotnet run --project API`
- Run tests: `dotnet test`
 
