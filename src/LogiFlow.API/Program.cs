using System.Text;
using LogiFlow.API.Middleware;
using LogiFlow.Application;
using LogiFlow.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// APPLICATION & INFRASTRUCTURE
// --------------------------------------------------

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


// --------------------------------------------------
// CONTROLLERS
// --------------------------------------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// --------------------------------------------------
// SWAGGER
// --------------------------------------------------

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LogiFlow Integrated Logistics API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {token}"
    });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});


// --------------------------------------------------
// JWT CONFIGURATION
// --------------------------------------------------

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Jwt:Key is missing from configuration.");

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "Jwt:Key must be at least 32 bytes long.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();


// --------------------------------------------------
// CORS
// --------------------------------------------------

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();


// --------------------------------------------------
// DATABASE SEEDING
// --------------------------------------------------

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<
            LogiFlow.Infrastructure.Data.ApplicationDbContext>();

    var hasher = scope.ServiceProvider
        .GetRequiredService<
            Microsoft.AspNetCore.Identity.IPasswordHasher<
                LogiFlow.Domain.Entities.User>>();

    await LogiFlow.Infrastructure.Data.DatabaseSeeder.SeedAsync(
        db,
        hasher,
        builder.Configuration["SeedAdmin:Email"]
            ?? "admin@logiflow.local",
        builder.Configuration["SeedAdmin:Password"]
            ?? "Admin@12345");
}


// --------------------------------------------------
// EXCEPTION HANDLING
// --------------------------------------------------

app.UseMiddleware<ExceptionHandlingMiddleware>();


// --------------------------------------------------
// SWAGGER
// --------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// --------------------------------------------------
// MIDDLEWARE
// --------------------------------------------------

app.UseHttpsRedirection();

app.UseCors("Angular");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();