# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS with-node
RUN apt-get update
RUN apt-get install curl
RUN curl -sL https://deb.nodesource.com/setup_20.x | bash
RUN apt-get -y install nodejs
RUN npm install -g @angular/cli

FROM with-node AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/NetCoreAngularApp.Template.Api/NetCoreAngularApp.Template.Api.csproj", "src/NetCoreAngularApp.Template.Api/"]
COPY ["src/NetCoreAngularApp.Template.AppHost/NetCoreAngularApp.Template.AppHost.csproj", "src/NetCoreAngularApp.Template.AppHost/"]
COPY ["src/NetCoreAngularApp.Template.Application/NetCoreAngularApp.Template.Application.csproj", "src/NetCoreAngularApp.Template.Application/"]
COPY ["src/NetCoreAngularApp.Template.Client/NetCoreAngularApp.Template.Client.esproj", "src/NetCoreAngularApp.Template.Client/"]
COPY ["src/NetCoreAngularApp.Template.Domain/NetCoreAngularApp.Template.Domain.csproj", "src/NetCoreAngularApp.Template.Domain/"]
COPY ["src/NetCoreAngularApp.Template.Infrastructure/NetCoreAngularApp.Template.Infrastructure.csproj", "src/NetCoreAngularApp.Template.Infrastructure/"]
COPY ["src/NetCoreAngularApp.Template.Migrations/NetCoreAngularApp.Template.Migrations.csproj", "src/NetCoreAngularApp.Template.Migrations/"]
COPY ["src/NetCoreAngularApp.Template.Persistence/NetCoreAngularApp.Template.Persistence.csproj", "src/NetCoreAngularApp.Template.Persistence/"]
COPY ["src/NetCoreAngularApp.Template.ServiceDefaults/NetCoreAngularApp.Template.ServiceDefaults.csproj", "src/NetCoreAngularApp.Template.ServiceDefaults/"]

RUN dotnet restore "./src/NetCoreAngularApp.Template.Api/NetCoreAngularApp.Template.Api.csproj"

COPY . .

WORKDIR "/src/src/NetCoreAngularApp.Template.Api"
RUN dotnet build "./NetCoreAngularApp.Template.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NetCoreAngularApp.Template.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

COPY --from=publish /src/src/NetCoreAngularApp.Template.Api/wwwroot ./wwwroot
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "NetCoreAngularApp.Template.Api.dll"]
