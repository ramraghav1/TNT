using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (!string.IsNullOrEmpty(databaseUrl) && databaseUrl.StartsWith("postgresql://"))
{
    // Convert URI format to Npgsql key-value format
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var host = uri.Host;
    var port = uri.Port > 0 ? uri.Port : 5432;
    var database = uri.AbsolutePath.TrimStart('/');
    var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
    var sslMode = query["sslmode"] ?? "require";
    
    connectionString = $"Host={host};Port={port};Database={database};Username={userInfo[0]};Password={userInfo[1]};SSL Mode={sslMode};Timeout=300;Command Timeout=300";
}
else
{
    connectionString = databaseUrl ?? "Host=localhost;Port=5432;Database=TNT;Username=postgres;Password=1234";
}

Console.WriteLine($"Connecting to: {connectionString.Split(";Password=")[0]}...");

var serviceProvider = new ServiceCollection()
    .AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddPostgres() // or .AddSqlServer()
        .WithGlobalConnectionString(connectionString)
        .ScanIn(typeof(Program).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole())
    .BuildServiceProvider();

using var scope = serviceProvider.CreateScope();
var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

// Check command line arguments
var cmdArgs = Environment.GetCommandLineArgs();
if (cmdArgs.Length > 1 && cmdArgs[1].ToLower() == "reset")
{
    Console.WriteLine("⚠️  RESET MODE: Dropping all tables and re-running migrations...");
    Console.WriteLine("Are you sure you want to drop  all tables? This will delete ALL data! (y/N): ");
    var confirm = Console.ReadLine()?.Trim().ToLower();
    
    if (confirm == "y" || confirm == "yes")
    {
        try
        {
            Console.WriteLine("🗑️  Dropping all tables with CASCADE...");
            
            // Get database connection to execute raw SQL
            using var conn = new Npgsql.NpgsqlConnection(connectionString);
            conn.Open();
            
            // Drop all tables with CASCADE (PostgreSQL specific)
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                DO $$ DECLARE
                    r RECORD;
                BEGIN
                    FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = 'public') LOOP
                        EXECUTE 'DROP TABLE IF EXISTS ' || quote_ident(r.tablename) || ' CASCADE';
                    END LOOP;
                END $$;";
            cmd.ExecuteNonQuery();
            
            Console.WriteLine("✅ All tables dropped successfully.");
            Console.WriteLine("🔄 Running all migrations...");
            
            runner.MigrateUp();
            
            Console.WriteLine("✅ All migrations completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error during migration: {ex.Message}");
            throw;
        }
    }
    else
    {
        Console.WriteLine("Reset cancelled.");
    }
}
else if (cmdArgs.Length > 1 && cmdArgs[1].ToLower() == "down")
{
    Console.WriteLine("Rolling back last migration...");
    runner.Rollback(1);
    Console.WriteLine("✅ Rolled back 1 migration.");
}
else
{
    // Normal migration up
    Console.WriteLine("Running migrations...");
    runner.MigrateUp();
    Console.WriteLine("✅ Migrations completed.");
}
