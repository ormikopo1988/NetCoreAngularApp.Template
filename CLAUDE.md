# CLAUDE.md

## Project Overview

Full-stack .NET 10 + Angular 19 template following Clean Architecture. PostgreSQL database, .NET Aspire orchestration, Azure deployment.

## Quick Reference

- **Solution**: `NetCoreAngularApp.Template.sln`
- **SDK**: .NET 10.0.100 (`global.json`), Node 24.x
- **Database**: PostgreSQL 17 (snake_case naming via EFCore.NamingConventions)
- **Package versions**: Centralized in `Directory.Packages.props`
- **Build strictness**: Warnings as errors, all analyzers enabled (`Directory.Build.props`)

## Architecture (Clean Architecture)

```
Api (Presentation) → Infrastructure → Application → Domain
                   → Persistence    → Application → Domain
```

- **Domain**: Entities, `IResult<T>`, `Error`, constants. No project dependencies.
- **Application**: Services, DTOs, mappers, pagination. Depends on Domain.
- **Persistence**: EF Core DbContext, configurations, interceptors. Depends on Application.
- **Infrastructure**: Identity (`IUser`), telemetry. Depends on Application.
- **Api**: Controllers, global exception handler, DI composition root. References all layers.
- **Client**: Angular SPA. Builds to `Api/wwwroot/`.

## Build & Test Commands

```bash
# Build entire solution
dotnet build NetCoreAngularApp.Template.sln

# Run all backend tests
dotnet test NetCoreAngularApp.Template.sln

# Run unit tests only (no Docker needed)
dotnet test tests/NetCoreAngularApp.Template.Api.Tests.Unit
dotnet test tests/NetCoreAngularApp.Template.Application.Tests.Unit

# Run integration tests (requires Docker for Testcontainers)
dotnet test tests/NetCoreAngularApp.Template.Application.Tests.Integration

# Angular
cd src/NetCoreAngularApp.Template.Client
npm install
npm run build
npm test

# EF Core migrations (from repo root)
dotnet ef migrations add <Name> --project src/NetCoreAngularApp.Template.Migrations --startup-project src/NetCoreAngularApp.Template.Api
dotnet ef database update --project src/NetCoreAngularApp.Template.Migrations --startup-project src/NetCoreAngularApp.Template.Api
```

## Coding Conventions

### C# Style (enforced via `.editorconfig` + analyzers)

- **File-scoped namespaces** (`namespace Foo;` not `namespace Foo { }`)
- **Braces required** (`csharp_prefer_braces = true:error`)
- **4-space indentation**, CRLF line endings
- **No `this.` qualifier** — enforced as error
- **Language keywords** over BCL types (e.g., `int` not `Int32`)
- **Pattern matching** preferred over `as`/`is` with null checks
- **Expression-bodied members** for properties, indexers, operators, lambdas, accessors
- **Block-bodied** for methods and constructors
- **Using directives** outside namespace, system usings first

### Naming

| Element | Convention | Example |
|---------|------------|---------|
| Private fields | `_camelCase` | `_logger`, `_applicationDbContext` |
| Parameters | `camelCase` | `queryOptions`, `ct` |
| CancellationToken param | Always `ct` | `CancellationToken ct` |
| Classes/Methods | `PascalCase` | `WeatherForecastService`, `GetAllAsync` |
| Interfaces | `I` + PascalCase | `IWeatherForecastService` |
| DTOs | `[Entity]Dto` | `WeatherForecastDto` |
| Mapper extensions | `[Entity]Extensions` | `WeatherForecastExtensions` |
| API routes | `api/kebab-case` | `api/weather-forecasts` |
| Database tables/columns | snake_case (automatic) | `weather_forecasts`, `temperature_c` |
| Test classes | `[ClassUnderTest]Tests` | `WeatherForecastsControllerTests` |
| Test methods | `Method_ShouldExpected_WhenCondition` | `GetAll_ShouldReturnForecasts_WhenCalled` |
| LogEventId constants | `PascalCase`, grouped by feature | `WeatherForecastServiceGetAllFailed` |

### Namespaces

Follow folder structure: `NetCoreAngularApp.Template.[Layer].[Feature].[Responsibility]`

Example: `NetCoreAngularApp.Template.Application.WeatherForecasts.Services`

## Key Patterns

### Result Pattern (not exceptions)

Services return `IResult<T>`, never throw for expected errors:

```csharp
// Success
return Results.Ok(data);

// Error (caught exception)
return Results.InternalServerError<T>(ex, LogEventId.EventName);
```

Factory methods: `Results.Ok()`, `Results.NotFound()`, `Results.ValidationFailed()`, `Results.InternalServerError()`, `Results.Unauthorized()`, `Results.BadGateway()`, `Results.Error()`

Controllers convert via `.AsActionResult()` extension → maps `ErrorCode` to HTTP status + ProblemDetails.

### DI Registration

Each layer exposes `Add[Layer]()` extension on `IServiceCollection` in a `DependencyInjection.cs` file. Composed in `Program.cs`:

```csharp
builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication()
    .AddPersistence(builder.Configuration)
    .AddInfrastructure();
```

### Service Layer

- Interface in `[Feature]/Interfaces/I[Feature]Service.cs`
- Implementation in `[Feature]/Services/[Feature]Service.cs`
- Inject `ILogger<T>` + `IApplicationDbContext`
- Async-first, accept `CancellationToken ct = default`
- Wrap in try-catch, log with `LogEventId`, return `IResult<T>`
- Registered as **Scoped**

### DTOs and Mapping

- DTOs in `[Feature]/Dtos/[Entity]Dto.cs`
- Manual mapping via extension methods in `[Feature]/Mappings/[Entity]Extensions.cs`
- Method naming: `To[DtoName]s()` (e.g., `ToWeatherForecastDtos()`)
- No AutoMapper — keep mappings explicit

### Pagination

- Request: `BasePaginationQuery` with `Page` and `PageSize` (both `required init`)
- Response: `PaginatedList<T>` with `Items`, `Page`, `PageSize`, `Total`, computed `HasNextPage`

### Entity Base Classes

- `BaseEntity`: `Guid Id` (auto-initialized)
- `BaseAuditableEntity` : `BaseEntity` + `Created`, `CreatedBy`, `LastModified`, `LastModifiedBy` (auto-populated by `AuditableEntityInterceptor`)

### Structured Logging

Define `EventId` constants in `Domain/Constants/LogEventId.cs`, grouped by feature with `#region`. Pass to both `ILogger` and `Results` factory.

### Error Responses

All errors follow RFC 7807 ProblemDetails with `traceId` and `eventId` in extensions.

## Testing

- **Framework**: xUnit v3 (`[Fact]`, `[Theory]`)
- **Mocking**: NSubstitute (`Substitute.For<T>()`)
- **Assertions**: FluentAssertions (`.Should().`)
- **SUT pattern**: `_sut` field for system under test
- **Arrange-Act-Assert** with section comments
- **Integration tests**: Testcontainers (PostgreSQL), `TestBase` base class, `IAsyncLifetime` for setup/teardown
- **CancellationToken**: Use `TestContext.Current.CancellationToken` in tests

## EF Core

- Abstract via `IApplicationDbContext` interface
- Entity configs in `Persistence/Configurations/[Entity]Configuration.cs` using `IEntityTypeConfiguration<T>`
- Migrations in separate `Migrations` project
- snake_case naming convention applied globally
- Query splitting: `SingleQuery` mode

## Angular (Client)

- NgModule-based (components use `standalone: false`)
- `provideHttpClient(withInterceptorsFromDi())` for API calls
- Proxy dev requests to .NET backend via `proxy.conf.js`
- Build output: `../NetCoreAngularApp.Template.Api/wwwroot`
- TypeScript interfaces mirror backend DTOs
- MSBuild intermediate output is redirected to `{repo_root}/obj/NetCoreAngularApp.Template.Client/` via `BaseIntermediateOutputPath` in the `.esproj` file. This prevents Angular 19's TypeScript compiler from encountering MSBuild artifacts (`obj/Debug`) during directory traversal, which would crash with a fatal `TS500 ENOENT` error. Do not remove this setting.

## Infrastructure

- **Aspire**: AppHost orchestrates API + Client + PostgreSQL + AppInsights for local dev
- **Docker Compose**: API, PostgreSQL 17, SonarQube (local quality analysis)
- **Azure**: Bicep IaC in `infra/` — App Service, PostgreSQL Flexible Server, Key Vault, Application Insights
- **CI/CD**: GitHub Actions — `build.yml` (CI + SonarCloud + Snyk), `azure-dev.yml` (CD), `codeql.yml` (security)
- **Health checks**: `/health` (full, includes DB), `/alive` (liveness)

## Version Sync Points

Some versions live in multiple places and must be bumped together. When changing one of these, update **all** the listed locations in the same commit:

| Concern | Locations |
|---|---|
| **EF Core runtime ↔ tooling** | `Directory.Packages.props` (`Microsoft.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.Tools`) **and** `.github/workflows/build.yml` (`dotnet tool install dotnet-ef --version <X.Y.Z>`) |
| **Node.js** | `.github/workflows/build.yml` (`node-version`), `src/NetCoreAngularApp.Template.Api/Dockerfile` (`setup_<X>.x`), `docs/tech-stack.md` (Frontend table), `docs/devops-and-infrastructure.md` (Dockerfile stage table) |
| **.NET SDK / target framework** | `global.json`, `Directory.Build.props` (`<TargetFramework>`), `src/NetCoreAngularApp.Template.Api/Dockerfile` (`dotnet/aspnet:<X.Y>`, `dotnet/sdk:<X.Y>`), `.github/workflows/build.yml` (`DOTNET_VERSION`) |
| **.NET Aspire** | All `Aspire.Hosting.*` entries in `Directory.Packages.props` must be the same version |

## Documentation Maintenance

**Keep documentation in sync with code changes.** When making changes that affect architecture, patterns, conventions, project structure, tech stack, or infrastructure:

1. Update the relevant `docs/*.md` file(s) to reflect the change.
2. Update this `CLAUDE.md` file if the change affects conventions, commands, patterns, or quick-reference information.
3. If a change introduces a new area not covered by existing docs, create a new `docs/<topic>.md` file and add it to the references below.

Documentation should stay concise and accurate — remove outdated information rather than letting it accumulate.

## References

Detailed documentation lives in the `docs/` folder:

- [`docs/project-structure.md`](docs/project-structure.md) — Solution layout, project reference graph, configuration files, Docker Compose services
- [`docs/architecture.md`](docs/architecture.md) — Clean Architecture layers, design patterns, data flow, error handling strategy, frontend-backend integration
- [`docs/tech-stack.md`](docs/tech-stack.md) — Complete technology inventory with versions (backend, frontend, database, Azure, observability, testing, CI/CD)
- [`docs/domain-and-features.md`](docs/domain-and-features.md) — Entity model, API endpoints, application services, Angular components, database schema and seed data, cross-cutting concerns
- [`docs/devops-and-infrastructure.md`](docs/devops-and-infrastructure.md) — CI/CD pipelines, Docker multi-stage build, Azure Bicep infrastructure, Aspire orchestration, health checks, code quality gates
