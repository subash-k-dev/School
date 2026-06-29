using Serilog;
using School.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using School.Application.Interfaces.Auth;
using School.Application.Interfaces.Repositories;
using School.Application.Services;
using School.Infrastructure.Auth;
using School.Infrastructure.Persistence;
using School.Infrastructure.Repositories;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ? Swagger with JWT Authorize button
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "School API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(cs));


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenService>(_ =>
{
    var cfg = builder.Configuration;
    return new TokenService(
        cfg["Jwt:Key"]!,
        cfg["Jwt:Issuer"]!,
        cfg["Jwt:Audience"]!
    );
});
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<TeacherService>();
builder.Services.AddScoped<IFeePaymentRepository, FeePaymentRepository>();
builder.Services.AddScoped<FeeService>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();

// JWT authentication
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// ? Seed Admin + Teacher
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasherService>();

    await School.Api.Seed.DbSeeder.SeedAsync(db, hasher);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();

// ? Global exception handler
app.UseGlobalExceptionMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting School API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}