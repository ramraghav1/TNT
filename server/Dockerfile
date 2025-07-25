# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY Solution.sln ./

# Copy all projects folders
COPY server/ server/
COPY Bussiness/ Bussiness/
COPY DbDeployment/ DbDeployment/
COPY Domain/ Domain/
COPY Repository/ Repository/

# Copy client folder ONLY if it is a .NET project referenced by the solution
# Otherwise exclude if client is Angular/React etc.
# COPY client/ client/

# Restore dependencies
RUN dotnet restore

# Copy all source code (if needed)
COPY . .

# Publish the Server project
WORKDIR /src/server
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "server.dll"]
