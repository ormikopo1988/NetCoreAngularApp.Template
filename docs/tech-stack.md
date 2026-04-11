# Technology Stack

## Backend

| Technology | Version | Purpose |
|-----------|---------|---------|
| .NET | 10.0 (SDK 10.0.100) | Runtime and SDK |
| ASP.NET Core | 10.0 | Web API framework |
| C# | 13 | Programming language |
| Entity Framework Core | 10.0.5 | ORM / data access |
| Npgsql | 10.0.1 | PostgreSQL EF Core provider |
| EFCore.NamingConventions | 10.0.1 | Snake_case database naming |

## Frontend

| Technology | Version | Purpose |
|-----------|---------|---------|
| Angular | 19.2.18 | SPA framework |
| Angular CLI | 19.2.20 | Build tooling |
| TypeScript | 5.7.3 | Language |
| RxJS | 7.8.2 | Reactive programming |
| Zone.js | 0.15.1 | Change detection |
| Node.js | 24.x | Runtime (build and dev server) |

## Database

| Technology | Version | Purpose |
|-----------|---------|---------|
| PostgreSQL | 17 | Primary database |
| EF Core Migrations | - | Schema versioning |

## Cloud & Azure

| Technology | Version | Purpose |
|-----------|---------|---------|
| Azure App Service | - | Application hosting |
| Azure PostgreSQL Flexible Server | - | Managed database |
| Azure Key Vault | - | Secrets management |
| Azure Application Insights | - | APM and telemetry |
| Azure Log Analytics | - | Centralized logging |
| Azure Bicep | - | Infrastructure as Code |
| Azure Developer CLI (azd) | - | Deployment orchestration |
| Azure.Identity | 1.20.0 | Managed identity auth |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.5.0 | Key Vault config provider |

## Observability

| Technology | Version | Purpose |
|-----------|---------|---------|
| OpenTelemetry (OTLP) | 1.15.2 | Distributed tracing and metrics |
| OpenTelemetry ASP.NET Core instrumentation | 1.15.1 | HTTP request tracing |
| OpenTelemetry HTTP instrumentation | 1.15.0 | Outbound HTTP tracing |
| OpenTelemetry Runtime instrumentation | 1.15.0 | Runtime metrics |
| Azure Monitor OpenTelemetry | 1.4.0 | Azure Monitor exporter (Application Insights) |

## Orchestration

| Technology | Version | Purpose |
|-----------|---------|---------|
| .NET Aspire | 13.2.2 | Local dev orchestration |
| Aspire.Hosting.PostgreSQL | 13.2.2 | DB provisioning |
| Aspire.Hosting.JavaScript | 13.2.2 | Angular dev server management |
| Aspire.Hosting.Azure.ApplicationInsights | 13.2.2 | Telemetry provisioning |

## Resilience & Service Discovery

| Technology | Version | Purpose |
|-----------|---------|---------|
| Microsoft.Extensions.Http.Resilience | 10.4.0 | HTTP resilience policies |
| Microsoft.Extensions.ServiceDiscovery | 10.4.0 | Service discovery |

## Testing (Backend)

| Technology | Version | Purpose |
|-----------|---------|---------|
| xUnit | 3.2.2 | Test framework |
| xunit.runner.visualstudio | 3.1.5 | Test runner |
| NSubstitute | 5.3.0 | Mocking |
| FluentAssertions | 8.9.0 | Assertion library |
| Microsoft.NET.Test.Sdk | 18.4.0 | Test SDK |
| coverlet.collector | 8.0.1 | Code coverage |
| Testcontainers | 4.11.0 | Docker-based integration tests |
| Microsoft.AspNetCore.Mvc.Testing | 10.0.5 | WebApplicationFactory |

## Testing (Frontend)

| Technology | Version | Purpose |
|-----------|---------|---------|
| Karma | 6.4.x | Test runner |
| Jasmine | 5.1.x | Test framework |
| karma-chrome-launcher | 3.2.x | Browser launcher |
| karma-coverage | 2.2.x | Coverage reporting |

## Code Quality

| Technology | Version | Purpose |
|-----------|---------|---------|
| SonarAnalyzer.CSharp | 10.23.0 | Static analysis (.NET) |
| SonarCloud | - | Continuous code quality |
| Snyk | - | Dependency vulnerability scanning |
| CodeQL | - | Security vulnerability scanning |

## Containerization

| Technology | Version | Purpose |
|-----------|---------|---------|
| Docker | - | Container runtime |
| Docker Compose | - | Multi-container orchestration |
| Multi-stage Dockerfile | - | Optimized image builds |

## CI/CD

| Technology | Purpose |
|-----------|---------|
| GitHub Actions | CI/CD pipeline |
| `build.yml` | Build, test, SonarCloud, Snyk |
| `azure-dev.yml` | Azure deployment with azd |
| `codeql.yml` | CodeQL security analysis |
| `jekyll-gh-pages.yml` | Documentation site |

## Development Tools

| Technology | Purpose |
|-----------|---------|
| Dev Containers | Reproducible dev environments |
| EditorConfig | Consistent code formatting |
| SonarLint | IDE-level code analysis |
| VS Code extensions | Azure, Bicep, Docker tooling |

## Package Management

- **NuGet**: Centralized version management via `Directory.Packages.props`
- **npm**: Angular dependencies via `package.json` / `package-lock.json`

## Authentication

The template includes a minimal identity abstraction (`IUser` / `CurrentUser`) but does not ship with a full authentication implementation (no Identity, JWT, or OAuth configured). This is intentional -- the template provides the extension point for teams to plug in their preferred auth mechanism.
