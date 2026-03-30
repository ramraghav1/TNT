# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first for layer caching
COPY Solution.sln ./
COPY server/server.csproj server/
COPY Bussiness/Bussiness.csproj Bussiness/
COPY DbDeployment/DbDeployment.csproj DbDeployment/
COPY Domain/Domain.csproj Domain/
COPY Repository/Repository.csproj Repository/
COPY TNT.Bussiness/TNT.Bussiness.csproj TNT.Bussiness/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Publish the Server project
WORKDIR /src/server
RUN dotnet publish -c Release -o /app/publish

# Publish the DbDeployment project (for running migrations)
WORKDIR /src/DbDeployment
RUN dotnet publish -c Release -o /app/migrations

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish .
COPY --from=build /app/migrations ./migrations/

# Render uses PORT env variable
ENV ASPNETCORE_URLS=http://+:${PORT:-10000}
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

# Run migrations then start the server
CMD dotnet migrations/DbDeployment.dll && dotnet server.dll
