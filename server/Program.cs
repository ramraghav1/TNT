using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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
var app = builder.Build();

// 2. Add Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<GlobalExceptionHandler>();
app.UseHttpsRedirection();

app.UseAuthentication(); // 👈 Add this before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
