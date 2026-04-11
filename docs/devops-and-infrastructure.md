# DevOps and Infrastructure

## CI/CD Pipelines (GitHub Actions)

### build.yml -- Continuous Integration

**Trigger**: Pull requests to `main`
**Runner**: `ubuntu-latest`

**Steps**:
1. Setup .NET 10.x SDK
2. Setup Java 17 (required for SonarCloud scanner)
3. Checkout repository
4. Cache SonarCloud packages and scanner
5. Install and run SonarCloud scanner (begin analysis)
6. Build solution
7. Run xUnit tests with coverage collection
8. Complete SonarCloud analysis
9. Run Snyk vulnerability scan
10. Build Angular frontend (`npm run build`)
11. Optionally publish build artifacts:
    - Website zip (published .NET app)
    - EF Core migrations bundle

> **Note — Angular build and MSBuild artifacts**: `dotnet build` processes the `.esproj` project and writes MSBuild intermediate files to `obj/` by default. Angular 19's TypeScript compiler crashes with a fatal `TS500 ENOENT` if it encounters those files during directory traversal. To prevent this, `BaseIntermediateOutputPath` is set in `NetCoreAngularApp.Template.Client.esproj` to redirect MSBuild output to `{repo_root}/obj/NetCoreAngularApp.Template.Client/`, keeping the Angular source tree clean regardless of dotnet build ordering.

### azure-dev.yml -- Continuous Deployment

**Trigger**: Push to `main` or manual workflow dispatch
**Runner**: `ubuntu-latest`

**Steps**:
1. Setup .NET SDK and Azure CLI
2. Login to Azure via OpenID Connect (federated credentials)
3. Provision infrastructure with `azd provision`
4. Deploy application with `azd deploy`

**Required Secrets**:
- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`
- `AZURE_CREDENTIALS`
- `AZURE_ENV_NAME`
- `AZURE_LOCATION`

### codeql.yml -- Security Scanning

**Trigger**: Pull requests and pushes to `main`
**Purpose**: CodeQL analysis for C# code to detect security vulnerabilities

### jekyll-gh-pages.yml -- Documentation

**Purpose**: Builds and deploys Jekyll documentation to GitHub Pages

## Docker

### Dockerfile (Multi-Stage Build)

**Location**: `src/NetCoreAngularApp.Template.Api/Dockerfile`

| Stage | Base Image | Purpose |
|-------|-----------|---------|
| `base` | `dotnet/aspnet:10.0` | Runtime base |
| `with-node` | `dotnet/sdk:10.0` | SDK + Node.js 24.x + Angular CLI |
| `build` | `with-node` | Restore, build, Angular compile |
| `publish` | `build` | `dotnet publish` release output |
| `final` | `base` | Production image |

### Docker Compose

**Services**:

| Service | Image | Ports | Purpose |
|---------|-------|-------|---------|
| `netcoreangularapp.template.api` | Built from Dockerfile | 5000 (HTTP), 5001 (HTTPS) | Application |
| `postgres` | `postgres:17` | 5432 | Application database |
| `sonarqube` | `sonarqube:community` | 9000 | Code quality portal |
| `sonarqube.db` | `postgres:17` | - | SonarQube database |

**Networks**:
- `ipv4` -- default, IPv4 only
- `dual` -- IPv4 + IPv6 support

**Volumes**:
- `.containers/db/` -- PostgreSQL init scripts
- `.containers/db_data/` -- PostgreSQL data persistence
- SonarQube volumes (data, extensions, logs, temp)
- ASP.NET HTTPS certificates (`~/.aspnet/https/`)

## Azure Infrastructure (Bicep)

### Resource Architecture

```
Subscription Scope (main.bicep)
└── Resource Group
    ├── Azure App Service Plan
    │   └── Azure App Service (API + Angular SPA)
    ├── Azure PostgreSQL Flexible Server
    │   └── Database: NetCoreAngularAppTemplateDb
    ├── Azure Key Vault
    │   └── Secrets (connection strings, etc.)
    ├── Azure Application Insights
    │   └── Dashboard
    └── Azure Log Analytics Workspace
```

### Bicep File Map

| File | Scope | Purpose |
|------|-------|---------|
| `infra/main.bicep` | Subscription | Root template, resource group creation |
| `infra/main.parameters.json` | - | Parameter values |
| `infra/core/host/appserviceplan.bicep` | Resource Group | App Service Plan definition |
| `infra/core/host/appservice.bicep` | Resource Group | App Service definition |
| `infra/core/host/appservice-appsettings.bicep` | Resource Group | App settings configuration |
| `infra/core/database/postgresql/flexibleserver.bicep` | Resource Group | PostgreSQL Flexible Server |
| `infra/core/security/keyvault.bicep` | Resource Group | Key Vault resource |
| `infra/core/security/keyvault-access.bicep` | Resource Group | Access policies |
| `infra/core/security/keyvault-secret.bicep` | Resource Group | Secret definitions |
| `infra/core/monitor/loganalytics.bicep` | Resource Group | Log Analytics workspace |
| `infra/core/monitor/applicationinsights.bicep` | Resource Group | Application Insights |
| `infra/core/monitor/applicationinsights-dashboard.bicep` | Resource Group | Portal dashboard |
| `infra/core/monitor/monitoring.bicep` | Resource Group | Monitoring orchestration |
| `infra/services/api.bicep` | Resource Group | API-specific infra |

### Azure Developer CLI (azd)

**Configuration** (`azure.yaml`):
```yaml
name: net-core-angular-app-azd
services:
  api:
    language: csharp
    project: ./src/NetCoreAngularApp.Template.Api
    host: appservice
```

## Development Environment

### Dev Container (`.devcontainer/devcontainer.json`)

- **Base**: `mcr.microsoft.com/devcontainers/python:3.10-bullseye`
- **Features**: Docker-in-Docker, Azure Developer CLI
- **VS Code Extensions**: GitHub Actions, Azure tools, Bicep, Docker
- **Minimum RAM**: 8 GB

### .NET Aspire (Local Orchestration)

The `AppHost` project orchestrates local development:
- Provisions a PostgreSQL container
- Configures Application Insights (optional)
- Starts the API and Angular dev server
- Provides a dashboard for service health and traces

### Health Checks

| Endpoint | Purpose |
|----------|---------|
| `/health` | Full readiness check (includes PostgreSQL connectivity) |
| `/alive` | Liveness probe (app is running) |

## Code Quality Gates

| Tool | Integration | Purpose |
|------|-------------|---------|
| SonarCloud | GitHub Actions (build.yml) | Continuous code quality and coverage |
| SonarQube | Docker Compose (local) | Local code quality analysis |
| SonarAnalyzer | Build-time (NuGet) | In-IDE static analysis |
| Snyk | GitHub Actions (build.yml) | Dependency vulnerability scanning |
| CodeQL | GitHub Actions (codeql.yml) | Security vulnerability detection |
| EditorConfig | IDE | Code formatting consistency |
| Warnings as Errors | Build (Directory.Build.props) | Strict compilation |
