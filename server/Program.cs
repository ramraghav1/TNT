using Business.Services;
using Bussiness.Services;
using Bussiness.Services.Organization;
using Bussiness.Services.Remittance;
using Bussiness.Services.TourAndTravels;
using Bussiness.Services.Transaction;
using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Repository.Dapper;
using Repository.Interfaces;
using Repository.Interfaces.Remittance;
using Repository.Interfaces.TourAndTravels;
using Repository.Repositories;
using Repository.Repositories.Remittance;
using Repository.Repositories.TourAndTravels;
using server.MiddleWare;
using System.Data;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;

using Dapper;

var builder = WebApplication.CreateBuilder(args);

// ────────────────────────────────────────────
// .NET 10 Best Practice: Configure JSON options globally
// ────────────────────────────────────────────
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Enable Dapper snake_case to PascalCase property mapping
DefaultTypeMap.MatchNamesWithUnderscores = true;

// Load JWT settings from configuration
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// ────────────────────────────────────────────
// Configure JWT Authentication
// ────────────────────────────────────────────
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddControllers();

// ────────────────────────────────────────────
// .NET 10: Built-in OpenAPI (replaces Swashbuckle)
// ────────────────────────────────────────────
builder.Services.AddOpenApi();

// ────────────────────────────────────────────
// .NET 10 Best Practice: ProblemDetails for consistent error responses
// ────────────────────────────────────────────
builder.Services.AddProblemDetails();

// ────────────────────────────────────────────
// .NET 10 Best Practice: IExceptionHandler for global error handling
// ────────────────────────────────────────────
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// ────────────────────────────────────────────
// .NET 10 Performance: Response Compression
// ────────────────────────────────────────────
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/json", "text/json"]);
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});

// ────────────────────────────────────────────
// .NET 10 Performance: Rate Limiting to protect against abuse
// ────────────────────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 10;
    });
});

// ────────────────────────────────────────────
// .NET 10 Performance: Output Caching for GET endpoints
// ────────────────────────────────────────────
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromSeconds(30)));
    options.AddPolicy("NoCache", builder => builder.NoCache());
    options.AddPolicy("ShortCache", builder => builder.Expire(TimeSpan.FromSeconds(10)));
    options.AddPolicy("LongCache", builder => builder.Expire(TimeSpan.FromMinutes(5)));
});

// ────────────────────────────────────────────
// .NET 10 Performance: Memory Cache + HybridCache for distributed scenarios
// ────────────────────────────────────────────
builder.Services.AddMemoryCache();
#pragma warning disable EXTEXP0018 // HybridCache is experimental in .NET 10
builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new Microsoft.Extensions.Caching.Hybrid.HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(10),
        LocalCacheExpiration = TimeSpan.FromMinutes(5)
    };
});
#pragma warning restore EXTEXP0018

// Register AutoMapper scanning all assemblies to find profiles
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Register Dapper DB Context as singleton (if thread-safe)
builder.Services.AddSingleton<IDapperDbContext, DapperDbContext>();

// ────────────────────────────────────────────
// Register PostgreSQL connection factory
// Support DATABASE_URL env var (Render/Neon) or fall back to appsettings
// ────────────────────────────────────────────
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;
if (!string.IsNullOrEmpty(databaseUrl) && databaseUrl.StartsWith("postgresql://"))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var host = uri.Host;
    var port = uri.Port > 0 ? uri.Port : 5432;
    var database = uri.AbsolutePath.TrimStart('/');
    connectionString = $"Host={host};Port={port};Database={database};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require";
}
else
{
    connectionString = databaseUrl ?? builder.Configuration.GetConnectionString("DefaultConnection")!;
}
builder.Services.AddTransient<IDbConnection>(sp => new NpgsqlConnection(connectionString));

// ────────────────────────────────────────────
// Register repository and service layers
// ────────────────────────────────────────────
builder.Services.AddScoped<IUserInformationRepository, UserInformationRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<IAuthTokenRepository, AuthTokenRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ICreateTransactionService, CreateTransactionService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IItineraryService, ItineraryService>();
builder.Services.AddScoped<IItineraryRepository, ItineraryRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Remittance module
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IPaymentTypeRepository, PaymentTypeRepository>();
builder.Services.AddScoped<IPaymentTypeService, PaymentTypeService>();
builder.Services.AddScoped<IAgentRepository, AgentRepository>();
builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddScoped<IServiceChargeSetupRepository, ServiceChargeSetupRepository>();
builder.Services.AddScoped<IServiceChargeSetupService, ServiceChargeSetupService>();
builder.Services.AddScoped<IFxRateSetupRepository, FxRateSetupRepository>();
builder.Services.AddScoped<IFxRateSetupService, FxRateSetupService>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IBranchUserRepository, BranchUserRepository>();
builder.Services.AddScoped<IBranchUserService, BranchUserService>();
builder.Services.AddScoped<IAdministrativeDivisionRepository, AdministrativeDivisionRepository>();
builder.Services.AddScoped<IAdministrativeDivisionService, AdministrativeDivisionService>();
builder.Services.AddScoped<IAgentAccountRepository, AgentAccountRepository>();
builder.Services.AddScoped<IAgentAccountService, AgentAccountService>();
builder.Services.AddScoped<IAgentLedgerEntryRepository, AgentLedgerEntryRepository>();
builder.Services.AddScoped<IAgentLedgerEntryService, AgentLedgerEntryService>();
builder.Services.AddScoped<IConfigurationTypeRepository, ConfigurationTypeRepository>();
builder.Services.AddScoped<IConfigurationTypeService, ConfigurationTypeService>();
builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<IDomesticServiceChargeSetupRepository, DomesticServiceChargeSetupRepository>();
builder.Services.AddScoped<IDomesticServiceChargeSetupService, DomesticServiceChargeSetupService>();

// ────────────────────────────────────────────
// Configure CORS
// ────────────────────────────────────────────
var allowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(',') 
    ?? ["http://localhost:4200", "https://localhost:4200"];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ────────────────────────────────────────────
// Middleware Pipeline (.NET 10 best-practice order)
// ────────────────────────────────────────────

// 1. Response Compression (must be first to compress all responses)
app.UseResponseCompression();

// 2. Exception Handler (ProblemDetails + IExceptionHandler)
app.UseExceptionHandler();

// 3. Only redirect to HTTPS in development (Render handles SSL at proxy level)
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// 4. Rate Limiting
app.UseRateLimiter();

// 5. CORS
app.UseCors();

// 6. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 7. Output Caching
app.UseOutputCache();

// 8. Map endpoints
app.MapControllers();

// 9. .NET 10: Built-in OpenAPI endpoint at /openapi/v1.json
app.MapOpenApi();

// 10. Scalar API Reference UI (replaces Swagger UI)
app.MapScalarApiReference();

app.Run();
