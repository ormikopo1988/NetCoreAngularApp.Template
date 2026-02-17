# Architecture

## Architectural Pattern: Clean Architecture

The codebase follows **Clean Architecture** (Onion Architecture) with strict dependency flow from outer layers inward. The Domain layer sits at the center with zero external project dependencies.

```
┌──────────────────────────────────────────────────────────┐
│  Presentation (Api)                                      │
│  ┌────────────────────────────────────────────────────┐  │
│  │  Infrastructure / Persistence                      │  │
│  │  ┌──────────────────────────────────────────────┐  │  │
│  │  │  Application                                 │  │  │
│  │  │  ┌────────────────────────────────────────┐  │  │  │
│  │  │  │  Domain (innermost, no dependencies)  │  │  │  │
│  │  │  └────────────────────────────────────────┘  │  │  │
│  │  └──────────────────────────────────────────────┘  │  │
│  └────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────┘
```

## Layer Responsibilities

### Domain (`NetCoreAngularApp.Template.Domain`)
- Entity definitions (`BaseEntity`, `BaseAuditableEntity`, `WeatherForecast`)
- Core types: `IResult<T>`, `Result<T>`, `Error`, `ErrorCode`
- Constants (`LogEventId`)
- No project references; depends only on `Microsoft.Extensions.Logging`

### Application (`NetCoreAngularApp.Template.Application`)
- Service interfaces and implementations (`IWeatherForecastService` / `WeatherForecastService`)
- DTOs (`WeatherForecastDto`)
- DTO mappers (extension methods)
- Pagination types (`PaginatedList<T>`, `BasePaginationQuery`)
- Depends on: **Domain**

### Persistence (`NetCoreAngularApp.Template.Persistence`)
- `ApplicationDbContext` (EF Core DbContext)
- `IApplicationDbContext` interface
- Entity configurations (Fluent API)
- Interceptors (`AuditableEntityInterceptor`)
- Database initialization and seeding (`ApplicationDbContextInitialiser`)
- Depends on: **Application**

### Infrastructure (`NetCoreAngularApp.Template.Infrastructure`)
- `IUser` / `CurrentUser` (identity abstraction)
- Application Insights telemetry initializer
- Azure Key Vault configuration
- Depends on: **Application**

### Api (`NetCoreAngularApp.Template.Api`)
- ASP.NET Core controllers (`WeatherForecastsController`)
- Global exception handler (`GlobalExceptionHandler`)
- Result-to-ActionResult extensions
- DI composition root (`Program.cs`)
- Depends on: **Infrastructure, Persistence, Migrations, ServiceDefaults, Client**

### Client (`NetCoreAngularApp.Template.Client`)
- Angular 17 SPA
- Builds to `wwwroot/` inside the Api project
- Communicates with backend via REST (`/api/*`)

### AppHost (`NetCoreAngularApp.Template.AppHost`)
- .NET Aspire orchestrator
- Provisions PostgreSQL and Application Insights for local dev
- Coordinates API and Client services

### ServiceDefaults (`NetCoreAngularApp.Template.ServiceDefaults`)
- Shared Aspire configuration (OpenTelemetry, health checks, service discovery, resilience)

## Design Patterns

### Result Pattern (Railway-Oriented Error Handling)
All service methods return `IResult<T>` instead of throwing exceptions. This provides explicit success/error paths:

```
IResult<T>
├── IsError: bool
├── Data: T?
└── Error: Error? (Code, Message, Exception)
```

Factory methods: `Results.Ok(value)`, `Results.NotFound(msg)`, `Results.InternalServerError(ex)`, etc.

`ResultExtensions.AsActionResult()` converts `IResult<T>` to the appropriate HTTP status code.

### DTO Mapping (Extension Methods)
Manual mapping via extension methods (e.g., `ToWeatherForecastDtos()`) rather than a mapping library like AutoMapper.

### EF Core Interceptor Pattern
`AuditableEntityInterceptor` hooks into `SaveChanges`/`SaveChangesAsync` to automatically populate `Created`, `CreatedBy`, `LastModified`, `LastModifiedBy` on entities that extend `BaseAuditableEntity`.

### Pagination
Standardized via `PaginatedList<T>` with `Items`, `Page`, `PageSize`, `Total`, and computed `HasNextPage`.

### Layered Dependency Injection
Each layer exposes an `Add*()` extension method. The API's `Program.cs` chains them:

```csharp
builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication()
    .AddPersistence(builder.Configuration)
    .AddInfrastructure(builder.Configuration);
```

### Entity Configuration (Fluent API)
EF Core entity configurations are separated into `IEntityTypeConfiguration<T>` classes, auto-applied from the Persistence assembly.

## Data Flow

```
Angular HttpClient
    │ GET /api/weather-forecasts?Page=1&PageSize=10
    ▼
WeatherForecastsController.GetAll()
    │ calls service
    ▼
WeatherForecastService.GetAllAsync()
    │ queries via IApplicationDbContext
    ▼
ApplicationDbContext (EF Core)
    │ Npgsql provider, snake_case naming
    ▼
PostgreSQL (weather_forecasts table)
    │ returns rows
    ▼
Entity → DTO mapping (ToWeatherForecastDtos)
    │ wrapped in PaginatedList<T>
    ▼
Results.Ok(paginatedList)
    │ IResult<T> → ActionResult<T>
    ▼
HTTP 200 JSON response
```

## Error Handling

1. **Service layer**: Catches exceptions, wraps in `Results.InternalServerError<T>(ex, eventId)`
2. **Controller layer**: `ResultExtensions.AsActionResult()` maps `ErrorCode` to HTTP status
3. **Global fallback**: `GlobalExceptionHandler` catches any unhandled exceptions and returns RFC 7807 Problem Details with a `traceId`

## Frontend-Backend Integration

- **Development**: Angular dev server (port 4200) proxies `/api/*` requests to .NET backend via `proxy.conf.js`
- **Production**: Angular builds to `wwwroot/` inside the API project. ASP.NET Core serves the SPA with `MapStaticAssets()` and `MapFallbackToFile("/index.html")`
- **Aspire**: AppHost orchestrates both services with automatic service discovery
