# Project Structure

## Solution: NetCoreAngularApp.Template.sln

The solution contains **11 projects** organized across source, test, and infrastructure concerns.

```
NetCoreAngularApp.Template/
├── src/                               # Application source code
│   ├── NetCoreAngularApp.Template.Api/              # ASP.NET Core Web API (entry point)
│   ├── NetCoreAngularApp.Template.Client/           # Angular 17 SPA (esproj)
│   ├── NetCoreAngularApp.Template.Domain/           # Domain entities and core types
│   ├── NetCoreAngularApp.Template.Application/      # Business logic and services
│   ├── NetCoreAngularApp.Template.Infrastructure/   # Cross-cutting concerns (telemetry, identity)
│   ├── NetCoreAngularApp.Template.Persistence/      # EF Core data access layer
│   ├── NetCoreAngularApp.Template.Migrations/       # EF Core database migrations
│   ├── NetCoreAngularApp.Template.ServiceDefaults/  # .NET Aspire shared service defaults
│   └── NetCoreAngularApp.Template.AppHost/          # .NET Aspire orchestrator
│
├── tests/                             # Test projects
│   ├── NetCoreAngularApp.Template.Api.Tests.Unit/              # API unit tests
│   ├── NetCoreAngularApp.Template.Application.Tests.Unit/      # Application unit tests
│   └── NetCoreAngularApp.Template.Application.Tests.Integration/ # Integration tests (Testcontainers)
│
├── infra/                             # Infrastructure as Code (Azure Bicep)
│   ├── main.bicep                     # Root Bicep template (subscription scope)
│   ├── main.parameters.json           # Parameter values
│   ├── abbreviations.json             # Azure resource naming abbreviations
│   ├── core/
│   │   ├── database/postgresql/       # PostgreSQL Flexible Server
│   │   ├── host/                      # App Service & App Service Plan
│   │   ├── monitor/                   # Application Insights, Log Analytics
│   │   └── security/                  # Key Vault, access policies, secrets
│   └── services/
│       └── api.bicep                  # API-specific infrastructure
│
├── .github/                           # GitHub configuration
│   ├── workflows/
│   │   ├── build.yml                  # CI: build, test, SonarCloud, Snyk
│   │   ├── azure-dev.yml             # CD: Azure deployment via azd
│   │   ├── codeql.yml                # Security: CodeQL scanning
│   │   └── jekyll-gh-pages.yml       # Docs: GitHub Pages
│   ├── pull_request_template.md
│   └── ISSUE_TEMPLATE/               # Bug report, feature request templates
│
├── .devcontainer/                     # Dev container configuration
│   └── devcontainer.json
│
├── .containers/                       # Docker volume data
│   ├── db/                            # PostgreSQL init scripts
│   └── db_data/                       # PostgreSQL persistent data
│
├── docker-compose.yml                 # Docker Compose: API, PostgreSQL, SonarQube
├── docker-compose.override.yml        # Local development overrides
├── docker-compose.dcproj              # Docker Compose project file
│
├── NetCoreAngularApp.Template.sln     # Visual Studio solution
├── global.json                        # .NET SDK version (10.0.100)
├── Directory.Build.props              # Global build properties (net10.0, analyzers)
├── Directory.Build.targets            # Global build targets
├── Directory.Packages.props           # Centralized NuGet package versions
├── azure.yaml                         # Azure Developer CLI config
├── nuget.config                       # NuGet sources
├── .editorconfig                      # Code style rules
├── .gitignore                         # Git ignore patterns
├── .dockerignore                      # Docker ignore patterns
├── README.md                          # Project documentation
└── LICENSE                            # License file
```

## Project Reference Graph

```
Api (Entry Point)
├── Client (Angular SPA)
├── Infrastructure
│   └── Application
│       └── Domain
├── Persistence
│   └── Application
│       └── Domain
├── Migrations
└── ServiceDefaults

AppHost (Aspire Orchestrator)
├── Coordinates Api
├── Coordinates Client
├── Provisions PostgreSQL
└── Provisions Application Insights
```

## Key Configuration Files

| File | Purpose |
|------|---------|
| `global.json` | Pins .NET SDK to 10.0.100 |
| `Directory.Build.props` | Sets TargetFramework (net10.0), enables nullable, treats warnings as errors |
| `Directory.Packages.props` | Central package version management (45+ packages) |
| `.editorconfig` | Code style and formatting rules |
| `azure.yaml` | Azure Developer CLI app definition (App Service host) |
| `nuget.config` | NuGet package source configuration |

## Docker Compose Services

| Service | Image | Port | Purpose |
|---------|-------|------|---------|
| `netcoreangularapp.template.api` | Custom (Dockerfile) | 5000/5001 | ASP.NET Core API + Angular SPA |
| `postgres` | postgres:17 | 5432 | Application database |
| `sonarqube` | sonarqube:community | 9000 | Code quality analysis |
| `sonarqube.db` | postgres:17 | - | SonarQube backend database |
