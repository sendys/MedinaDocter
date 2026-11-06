using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;
using MedinaDokter.Api.Features.Auth.Login;
using MedinaDokter.Api.Infrastructure.Persistence;
using MedinaDokter.Api.Infrastructure.Auth;
using MedinaDokter.Api.Features.Health;
using MedinaDokter.Api.Features.Auth.Me;
using MedinaDokter.Api.Features.Dokter.Me;
using MedinaDokter.Api.Common.Exceptions;
using MedinaDokter.Api.Common.Responses;
using System.Security.Claims;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. DATABASE CONFIGURATION
// ==========================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
     throw new InvalidOperationException(
         "ConnectionStrings:DefaultConnection is missing.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Authentication Service
builder.Services.AddScoped<IAuthService, AuthService>();

// ==========================================
// 2. JWT AUTHENTICATION CONFIGURATION
// ==========================================
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is missing");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is missing");

builder.Services.AddAuthentication(options =>
{
     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
     options.TokenValidationParameters = new TokenValidationParameters
     {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = jwtIssuer,
          ValidAudience = jwtAudience,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
          RoleClaimType = ClaimTypes.Role,
          ClockSkew = TimeSpan.Zero
     };

     options.Events = new JwtBearerEvents
     {
          // HTTP 401 - Unauthorized
          OnChallenge = async context =>
          {
               context.HandleResponse();

               context.Response.StatusCode =
                 StatusCodes.Status401Unauthorized;

               await context.Response.WriteAsJsonAsync(
                 ApiResponse<object>.Fail(
                     "Token tidak valid atau sudah kedaluwarsa."));
          },

          // HTTP 403 - Forbidden
          OnForbidden = async context =>
          {
               context.Response.StatusCode =
                 StatusCodes.Status403Forbidden;

               await context.Response.WriteAsJsonAsync(
                 ApiResponse<object>.Fail(
                     "Anda tidak memiliki hak akses untuk resource ini."));
          }
     };
});

builder.Services.AddAuthorization(options =>
{
     // Dokter dan admin boleh mengakses aplikasi
     options.AddPolicy("MedinaAccess", policy =>
         policy.RequireRole("dokter", "admin"));

     // Khusus dokter
     options.AddPolicy("DokterOnly", policy =>
         policy.RequireRole("dokter"));

     // Khusus admin
     options.AddPolicy("AdminOnly", policy =>
         policy.RequireRole("admin"));
});


// ==========================================
// 3. RATE LIMITING CONFIGURATION (.NET 7/8/9)
// ==========================================
builder.Services.AddRateLimiter(options =>
{
     options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
         RateLimitPartition.GetFixedWindowLimiter(
             partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
             factory: _ => new FixedWindowRateLimiterOptions
             {
                  AutoReplenishment = true,
                  PermitLimit = 100, // Maximum 100 requests
                  QueueLimit = 0,
                  Window = TimeSpan.FromMinutes(1) // per 1 minute
             }));

     options.OnRejected = async (context, cancellationToken) =>
     {
          context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
          await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);
     };
});


// ==========================================
// 4. CONFIGURATION FLUENT VALIDATION
// ==========================================
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// ==========================================
// 5. GLOBAL EXCEPTION HANDLING
// ==========================================
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ==========================================
// 6. MVC & CONTROLLERS
// ==========================================
builder.Services.AddControllers();


var app = builder.Build();


// ==========================================
// 7. HTTP REQUEST PIPELINE (MIDDLEWARE)
// ==========================================

app.UseExceptionHandler();

// Use Rate Limiter
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapDatabaseHealthEndpoint();
app.MapLoginEndpoint();
app.MapDatabaseMeEndpoint();
app.MapDokterMeEndpoint();

app.Run();