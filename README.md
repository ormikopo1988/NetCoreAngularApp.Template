[![Build](https://github.com/ormikopo1988/NetCoreAngularApp.Template/actions/workflows/build.yml/badge.svg)](https://github.com/ormikopo1988/NetCoreAngularApp.Template/actions/workflows/build.yml)
[![CodeQL](https://github.com/ormikopo1988/NetCoreAngularApp.Template/actions/workflows/codeql.yml/badge.svg)](https://github.com/ormikopo1988/NetCoreAngularApp.Template/actions/workflows/codeql.yml)
[![Azure Deployment](https://github.com/ormikopo1988/NetCoreAngularApp.Template/actions/workflows/azure-dev.yml/badge.svg)](https://github.com/ormikopo1988/NetCoreAngularApp.Template/actions/workflows/azure-dev.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=ormikopo1988_NetCoreAngularApp.Template&metric=alert_status&token=6306c5cf3f711bfaf03d114ba50463a61cbf8622)](https://sonarcloud.io/summary/new_code?id=ormikopo1988_NetCoreAngularApp.Template)

# NetCoreAngularApp.Template

## Overview

This solution template provides a comprehensive setup for developing, testing, and deploying applications with a focus on DevOps practices, including infrastructure as code (IaC), and continuous integration / continuous deployment (CI / CD). 

The template includes various components and configurations to streamline the development process, ensure best practices and demonstrate how to set up a .NET project the right way from day one. 

It covers everything from basic setup to tools like using a `.editorconfig` file to keep your code clean, managing all your packages in one place, and catching bugs early with code analysis. 

It also shows you how to package your app with Docker, orchestrate it using Docker Compose or .NET Aspire.

Finally, it shows you how to automate your builds and deployments with GitHub Actions and how to automate the infrastructure provisioning and deployment of the app using Azure Developer CLI (azd).

To sum up, on a high-level, this template includes the following:

* Basic .NET API with Angular SPA project setup using Clean Architecture.
* Integration tests setup
* Database migrations & seeding
* Code style (i.e., `.editorconfig`)
* Build configuration (i.e., `Directory.Build.props` and `Directory.Build.targets`)
* Central package management (i.e., `Directory.Packages.props`)
* Code quality: Static code analysis
* Containerization (i.e., `Dockerfile`)
* Orchestration: 
  * Docker Compose (i.e., `docker-compose.yml`)
  * .NET Aspire (i.e., `NetCoreAngularApp.Template.AppHost`)
* CI / CD pipelines through GitHub Actions
* Provisioning infrastructure and deploying the app using Azure Developer CLI (azd)
* SonarQube / SonarCloud integration
* Security scanning with Snyk

## Prerequisites

### Local machine requirements 

- [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

- [Docker](https://docs.docker.com/desktop/)

### Set up database

> Install [dotnet ef tool](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

Go to the root folder of the project and run the following command to **update database** with the current migrations.:

``` 
dotnet ef database update --project src\NetCoreAngularApp.Template.Migrations --startup-project src\NetCoreAngularApp.Template.Api
```

### Database commands

**Add migrations** when you need to apply new migration:

```
dotnet ef migrations add "MyMigration" --project src\NetCoreAngularApp.Template.Migrations --startup-project src\NetCoreAngularApp.Template.Api
```

**Update database** with the current migrations:

```
dotnet ef database update --project src\NetCoreAngularApp.Template.Migrations --startup-project src\NetCoreAngularApp.Template.Api
```

**Generate migration script**:

```
dotnet ef migrations script PreviousAppliedMigration --project src\NetCoreAngularApp.Template.Migrations --startup-project src\NetCoreAngularApp.Template.Api
```

**Drop database**:

```
dotnet ef database drop --project src\\NetCoreAngularApp.Template.Migrations --startup-project src\\NetCoreAngularApp.Template.Api
```

## Code

* **Solution Structure**: The solution follows the Clean Architecture structure and is organized into multiple projects, including:
    * `NetCoreAngularApp.Template.Api`: The main .NET 9 API project.
    * `NetCoreAngularApp.Template.Client`: The client project (Angular SPA).
    * `NetCoreAngularApp.Template.Domain`: A .NET 9 library project that contains the domain entities of the system.
    * `NetCoreAngularApp.Template.Application`: A .NET 9 library project that contains all application logic.
    * `NetCoreAngularApp.Template.Infrastructure`: A .NET 9 library project that contains classes for accessing external resources, such as file systems, web services, identity, and so on. These classes should be based on interfaces defined within the application layer.
    * `NetCoreAngularApp.Template.Persistence`: A .NET 9 library project that contains the EF Core entity configurations and data access specific code. It also contains the logic for the app database seeding.
    * `NetCoreAngularApp.Template.Migrations`: A .NET 9 library project that contains the Entity Framework migrations.

## Tests

* **Integration Tests**: The template includes integration tests to ensure code quality and functionality. These tests are also integrated as part of the CI pipeline.

## Running the Solution

### Visual Studio (F5)

* Open the solution in Visual Studio.
* Set `NetCoreAngularApp.Template.Api` as the startup project.
* Press F5 to build and run the solution.

### Docker Compose

* The template includes a `docker-compose.yml` file for running the solution in Docker containers.
* To start the solution using Docker Compose, run the following command in the terminal (you have to have `Docker` installed on your local machine):

```
docker-compose up --build
```

#### Local SonarQube Server

* The `docker-compose.yml` file also includes support for running a local SonarQube server.
* You must have installed the `dotnet-sonarscanner` and `dotnet-coverage` tools on your local machine (once-off): 
```
dotnet tool install --global dotnet-sonarscanner
dotnet tool install --global dotnet-coverage
```
* After starting the solution with Docker Compose, you can access the SonarQube server at `http://localhost:9000`.
* After logging in with the admin credentials (initial `admin` / `admin` and then you will be prompted to update the default password for user `admin`) you click on `Create a local project`, you then create a project called `netcoreangularapp-template` and finally generate a `sonar.token` project token.
* To conduct a Sonar Analysis locally, from the root folder of the Git repo, you must run the following commands:
```
dotnet sonarscanner begin /k:"netcoreangularapp-template" /d:sonar.host.url="http://localhost:9000" /d:sonar.token="{yourSonarToken}" /d:sonar.cs.vscoveragexml.reportsPaths=coverage.xml /d:sonar.exclusions=infra/core/**

dotnet build .\NetCoreAngularApp.Template.sln --no-incremental

dotnet-coverage collect "dotnet test NetCoreAngularApp.Template.sln" -f xml -o "coverage.xml"

dotnet sonarscanner end /d:sonar.token="{yourSonarToken}"
```

### .NET Aspire

* The solution can also be run using .NET Aspire, by setting the `NetCoreAngularApp.Template.AppHost` project as startup project and running it.

## DevOps (CI/CD/IaC)

### Continuous Integration and Continuous Deployment (CI/CD)

* **build.yml**: The template includes a GitHub Actions workflow (`.github/workflows/build.yml`) for CI. This workflow handles:
    * Running tests and building the solution.
    * Analyzing code quality with SonarQube.
    * Security scanning with Snyk.

* **azure-dev.yml**: The template includes a GitHub Actions workflow (`.github/workflows/azure-dev.yml`) for CD. This workflow handles:
    * Running the `build.yml` to validate the code and publish application and db migration artifacts.
    * Run preflight checks to ensure the production environment is ready for deployment.
    * For non-production environments, run a `what-if` analysis to determine the changes that will be made to the infrastructure.
    * Provision the necessary infrastructure, run DB migrations and deploy the application to Azure.

### Infrastructure as Code (IaC)

* **Bicep Templates**: The template uses Bicep for defining and deploying Azure resources.

### Code Analysis

* The GitHub Actions workflow integrates with **SonarCloud** for code quality analysis and **Snyk** for security scanning.
* These tools are configured to run as part of the CI pipeline, ensuring continuous monitoring of code quality and security.

## Provisioning Infrastructure and Deploying the App using Azure Developer CLI (azd)

### From your local machine

The steps in this section demonstrate how to handle 
provisioning of the necessary infrastructure and deploying of the app to Azure using `azd`:

* You have installed `azd` on your local machine.
* **Important**: If you are on Windows, and your user has an extra whitespace character, use the following command
through a PowerShell terminal before continuing. You can find more details in [this GitHub issue](https://github.com/Azure-Samples/azure-search-openai-demo/issues/502):

```
$env:AZD_CONFIG_DIR="C:\azdConfig"
```

- Execute the `azd init` command to initialize your project with `azd`, 
which will inspect the local directory structure and determine the type of app:

```
azd init
```

* Select **Use code in the current directory** when `azd` prompts you.
* After scanning the directory, `azd` prompts you to confirm that it found the correct 
.NET project. Select the **Confirm and continue initializing my app** option.

* Finally, specify the the environment name, which is used to name provisioned resources
in Azure and managing different environments such as `netcoreangularapp-template-dev`:
  * `azd` generates a number of files and places them into the working directory. These files are:
	* *azure.yaml*: Describes the services of the app, such as `NetCoreAngularApp.Template.Api` project, and maps them to Azure resources.
	* *.azure/config.json*: Configuration file that informs `azd` what the current active environment is.
	* *.azure/netcoreangularapp-template-dev/.env*: Contains environment specific overrides.
	* *.azure/netcoreangularapp-template-dev/config.json*: Configuration file that informs `azd` which services should have a public endpoint in this environment.
* The *azure.yaml* file has a `project` field pointing to the `NetCoreAngularApp.Template.Api` project. 
With this, `azd` activates its integration with Azure and derives the required infrastructure needed to host this application.
* The *.azure\netcoreangularapp-template-dev\config.json* file is how `azd` remembers 
(on a per environment basis) which services should be exposed with a public endpoint. 
`azd` can be configured to support multiple environments.

In order to deploy the application, authenticate to Azure AD 
to call the Azure resource management APIs.

```
azd auth login
```

The previous command will launch a browser to authenticate the command-line session.

Once authenticated, use the following command to provision and deploy the application.

```
azd up
```

When prompted, select the subscription and location the resources should be deployed to. 
Once these options are selected the application will be deployed.

Because `azd` knows the resource group in which it created the resources it can
be used to spin down the environment using the following command:

```
azd down --force --purge
```

### Configure a pipeline and push updates

You can also setup the Azure Developer CLI (azd) to push template changes through a CI/CD pipeline such as GitHub Actions or Azure DevOps.

azd templates may or may not include a default GitHub Actions and/or Azure DevOps pipeline configuration file called `azure-dev.yml`, which is required to setup CI/CD. 

This configuration file provisions your Azure resources and deploy your code to the main branch. 

You can find `azure-dev.yml`:

* For GitHub Actions: in the `.github/workflows` directory.
* For Azure DevOps: in the `.azuredevops/pipelines` directory or the `.azdo/pipelines` directory.

You can use the configuration file as-is or modify it to suit your needs.

> Make sure your template has a pipeline definition (`azure-dev.yaml`) before calling `azd pipeline config`, as azd does not automatically create this file.

Use the `azd pipeline config` command to configure a CI/CD pipeline, which handles the following tasks:

* Creates and configures a service principal for the app on the Azure subscription. Your user must have either Owner role or Contributor + User Access Administrator roles within the Azure subscription to allow azd to create and assign roles to the service principal.
* Steps you through a workflow to create and configure a GitHub or Azure DevOps repository and commit your project code to it. You can also choose to use an existing repository.
* Creates a secure connection between Azure and your repository.
* Runs the GitHub action when you check in the workflow file.

#### Authorize GitHub to deploy to Azure

To configure the workflow, you need to authorize a service principal to deploy to Azure on your behalf, from a GitHub action. azd creates the service principal and a federated credential for it.

Run the following command to create the Azure service principal and configure the pipeline:

```
azd pipeline config
```

> By default, azd pipeline config uses OpenID Connect (OIDC), called federated credentials. If you'd rather not use OIDC, run `azd pipeline config --auth-type client-credentials`.

Supply the requested GitHub information.

When prompted about committing and pushing your local changes to start a new GitHub Actions run, specify `Y`.

In the terminal window, view the results of the `azd pipeline config` command. 

Using your browser, open the GitHub repository for your project.

Select Actions to see the workflow running.

For the azure-dev.yml workflow to run successfully and run the preflight validations and what-if commands,
you need to setup the following secrets inside your GitHub repository:

* `AZURE_SQL_ADMINISTRATOR_PASSWORD`
* `AZURE_SQL_USER_PASSWORD`
* `DB_CONNECTION_STRING`
* `SNYK_TOKEN`
* `SONAR_TOKEN`

The `SNYK_TOKEN` and `SONAR_TOKEN` tokens can be generated from the [Snyk](https://app.snyk.io/account) and [SonarCloud](https://sonarcloud.io/account/security) websites respectively.

The `AZURE_SQL_ADMINISTRATOR_PASSWORD` & `AZURE_SQL_USER_PASSWORD` are the passwords for the Postgresql Server administrator and user accounts respectively.

The `DB_CONNECTION_STRING` is the connection string to the Postgresql database (e.g., `Server={hostname}.postgres.database.azure.com;Port=5432;Database={dbName};Username={dbUsername};Password={dbPassword}`).

## Resources

- https://learn.microsoft.com/en-us/dotnet/aspire/deployment/azure/aca-deployment
- https://learn.microsoft.com/en-us/dotnet/aspire/deployment/azure/aca-deployment-github-actions
- https://learn.microsoft.com/en-us/azure/developer/azure-developer-cli/configure-devops-pipeline
- https://learn.microsoft.com/en-us/dotnet/aspire/deployment/azure/application-insights
- https://docs.sonarsource.com/sonarqube-server/latest/analyzing-source-code/dotnet-environments/overview/
- https://docs.sonarsource.com/sonarqube-cloud/advanced-setup/ci-based-analysis/azure-pipelines/adding-analysis-to-build-pipeline/dotnet-project/
- https://github.com/snyk/actions/tree/master/dotnet