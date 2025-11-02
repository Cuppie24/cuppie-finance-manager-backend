# Base runtime layer
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Build layer
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY Domain/Domain.csproj Domain/
COPY Application/Application.csproj Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY CFinanceManager/CFinanceManager.csproj CFinanceManager/

# Restore dependencies
RUN dotnet restore CFinanceManager/CFinanceManager.csproj

# Copy remaining source code
COPY . .

# Build
WORKDIR /src/CFinanceManager
RUN dotnet build "CFinanceManager.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "CFinanceManager.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final container
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5295
ENTRYPOINT ["dotnet", "CFinanceManager.dll"]
