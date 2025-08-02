using Bussiness.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Repository.Dapper;
using Repository.Interfaces;
using Repository.Repositories;
using System.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAutoMapper(typeof(Program));
// 1. Add Authentication + JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = "TNT-server",
            ValidAudience = "TNT-users",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("s3cr3tK3y!@#2025-Super$LongAndRandomKey1234")) // Use a strong key!
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt")
);
builder.Services.AddSingleton<IDapperDbContext, DapperDbContext>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddAutoMapper(typeof(UserInformationProfile));
// Register IDbConnection as transient (new connection each time)
builder.Services.AddTransient<IDbConnection>(sp => new NpgsqlConnection(connectionString));

// Register your repository and service
builder.Services.AddScoped<IUserInformationRepository, UserInformationRepository>();
builder.Services.AddTransient<IUserService, UserService>();

var app = builder.Build();

// 2. Add Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<GlobalExceptionHandler>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
