#!/bin/sh
set -e

echo "▶ Running database migrations..."
dotnet /app/migrations/DbDeployment.dll

echo "✅ Migrations complete. Starting server..."
exec dotnet /app/server/server.dll
