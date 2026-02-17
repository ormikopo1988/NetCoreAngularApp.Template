# Domain Model and Features

## Domain Entities

### Entity Hierarchy

```
BaseEntity (abstract)
├── Guid Id (auto-generated)
│
└── BaseAuditableEntity (abstract)
    ├── DateTimeOffset Created
    ├── string? CreatedBy
    ├── DateTimeOffset LastModified
    └── string? LastModifiedBy
```

### WeatherForecast

**Inherits from**: `BaseEntity`
**Location**: `src/NetCoreAngularApp.Template.Domain/Entities/WeatherForecast.cs`

| Property | Type | Persisted | Notes |
|----------|------|-----------|-------|
| Id | Guid | Yes | From BaseEntity, auto-generated |
| Date | DateOnly | Yes | Forecast date |
| TemperatureC | int | Yes | Temperature in Celsius |
| TemperatureF | int | No | Computed: `32 + (int)(TemperatureC / 0.5556)` |
| Summary | string? | Yes | Description (e.g., "Warm", "Cold") |

**EF Core Configuration** (`WeatherForecastConfiguration`): Ignores the computed `TemperatureF` property so it is not mapped to the database.

## API Endpoints

### Weather Forecasts

| Method | Route | Query Params | Response |
|--------|-------|-------------|----------|
| GET | `/api/weather-forecasts` | `page` (default 1), `pageSize` (default 10) | `PaginatedList<WeatherForecastDto>` |

**Success Response (200):**
```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2026-02-17",
      "temperatureC": 20,
      "temperatureF": 68,
      "summary": "Warm"
    }
  ],
  "page": 1,
  "pageSize": 10,
  "total": 5,
  "hasNextPage": false
}
```

**Error Response (500, RFC 7807 Problem Details):**
```json
{
  "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
  "title": "InternalServerError",
  "status": 500,
  "detail": "Unhandled error",
  "extensions": {
    "eventId": 10,
    "traceId": "0HN1ABCDEF1G1K:00000001"
  }
}
```

### Health Endpoints

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/health` | Full health check (includes database) |
| GET | `/alive` | Liveness probe |

## Application Services

### IWeatherForecastService

**Location**: `src/NetCoreAngularApp.Template.Application/WeatherForecasts/Interfaces/IWeatherForecastService.cs`

| Method | Parameters | Returns |
|--------|------------|---------|
| `GetAllAsync` | `BasePaginationQuery`, `CancellationToken` | `Task<IResult<PaginatedList<WeatherForecastDto>>>` |

**Implementation** (`WeatherForecastService`):
1. Counts total forecasts in the database
2. Applies pagination (Skip/Take)
3. Maps entities to DTOs via `ToWeatherForecastDtos()` extension
4. Wraps in `PaginatedList<T>` and returns `Results.Ok(...)`
5. On exception: logs with `LogEventId.WeatherForecastServiceGetAllFailed` (EventId 10) and returns `Results.InternalServerError<T>`

## Angular Frontend

### Components

**AppComponent** (`src/NetCoreAngularApp.Template.Client/src/app/app.component.ts`):
- Root application component
- Fetches weather forecasts on initialization via `HttpClient`
- Displays forecasts in an HTML table (Date, Temp C, Temp F, Summary)
- Shows loading indicator during fetch

### Modules

| Module | Purpose |
|--------|---------|
| `AppModule` | Root module, bootstraps AppComponent |
| `AppRoutingModule` | Routing configuration (currently empty) |

### TypeScript Interfaces

```typescript
interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

interface PaginatedList<T> {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
  hasNextPage: boolean;
}
```

## Database

### Database: PostgreSQL 17

**Connection** (development): `Server=localhost;Port=5432;Database=NetCoreAngularAppTemplateDb;Username=postgres;Password=postgres`

**Naming Convention**: snake_case (table: `weather_forecasts`)

### Seed Data

`ApplicationDbContextInitialiser.SeedAsync()` inserts 5 weather forecasts on first run:

| Date | Temp (C) | Summary |
|------|----------|---------|
| Tomorrow + 1 | 20 | "Warm" |
| Tomorrow + 2 | 10 | "Cold" |
| Tomorrow + 3 | 0 | "Freezing" |
| Tomorrow + 4 | 30 | "Hot" |
| Tomorrow + 5 | 12 | "Cool" |

## Cross-Cutting Concerns

### Audit Trail
- `AuditableEntityInterceptor` (EF Core `SaveChangesInterceptor`)
- Automatically populates `Created`/`CreatedBy` on insert and `LastModified`/`LastModifiedBy` on update
- Reads current user from `IUser` and current time from `TimeProvider`
- Currently, `WeatherForecast` does not use `BaseAuditableEntity`, so auditing does not apply to it

### Global Exception Handling
- `GlobalExceptionHandler` implements `IExceptionHandler`
- Catches all unhandled exceptions
- Returns RFC 7807 Problem Details with `traceId` for correlation

### CORS
- Origins configured from `appsettings.json` `AllowCors` section
- Allows any header and credentials

### Identity
- Minimal abstraction: `IUser` interface with `Id` property
- `CurrentUser` implementation reads from `IHttpContextAccessor`
- No full authentication system is configured -- this is a placeholder for teams to add their own

## Background Jobs / Scheduled Tasks

None. The template does not include background processing. Database initialization and seeding happen at application startup.
