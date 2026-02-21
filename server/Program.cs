using Business.Services;
using Bussiness.Services;
using Bussiness.Services.Organization;
using Bussiness.Services.TourAndTravels;
using Bussiness.Services.Transaction;
using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Repository.Dapper;
using Repository.Interfaces;
using Repository.Interfaces.TourAndTravels;
using Repository.Repositories;
using Repository.Repositories.TourAndTravels;
using System.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load JWT settings from configuration
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Configure JWT Authentication
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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register AutoMapper scanning all assemblies to find profiles
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Register Dapper DB Context as singleton (if thread-safe)
builder.Services.AddSingleton<IDapperDbContext, DapperDbContext>();

// Register PostgreSQL connection factory as transient (new connection per request)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddTransient<IDbConnection>(sp => new NpgsqlConnection(connectionString));

// Register repository and service layers
builder.Services.AddScoped<IUserInformationRepository, UserInformationRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ILoginService, LoginService>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddTransient<ICreateTransactionService, CreateTransactionService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddTransient<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IItineraryService, ItineraryService>();
builder.Services.AddScoped<IItineraryRepository, ItineraryRepository>();
// Optional: Configure CORS (update policies as needed)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Enable Swagger middleware in dev or prod as needed
app.UseSwagger();
app.UseSwaggerUI();

// Global error handler middleware
app.UseMiddleware<GlobalExceptionHandler>();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Optional: Validate AutoMapper configuration at startup (uncomment for debugging)
// var mapper = app.Services.GetRequiredService<IMapper>();
// mapper.ConfigurationProvider.AssertConfigurationIsValid();

app.Run();
