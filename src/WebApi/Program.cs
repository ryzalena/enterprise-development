using Infrastructure.Data;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Регистрация сервисов
var services = builder.Services;

services.AddControllers();

// База данных
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=localhost,1433;Database=PolyclinicDB;User Id=sa;Password=MySecurePassword123!;TrustServerCertificate=True;";

// РЕГИСТРАЦИЯ DbContext
services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});

services.AddScoped<IPatientRepository, PatientRepository>();
services.AddScoped<IDoctorRepository, DoctorRepository>();
services.AddScoped<ISpecializationRepository, SpecializationRepository>();
services.AddScoped<IAppointmentRepository, AppointmentRepository>();

services.AddHostedService<NatsBackgroundService>();

services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRouting();

app.MapControllers();

app.MapGet("/", () => "Polyclinic Web API is running.");
app.MapGet("/health", () => Results.Ok(new 
{ 
    Status = "Healthy", 
    Timestamp = DateTime.UtcNow 
}));

app.MapGet("/health/nats", async (IServiceProvider services) =>
{
    try
    {
        using var scope = services.CreateScope();
        var natsService = scope.ServiceProvider.GetService<NatsBackgroundService>();
        return Results.Ok(new { 
            Status = "NATS Consumer Running",
            ServiceType = natsService?.GetType().Name ?? "Not registered"
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"NATS check failed: {ex.Message}");
    }
});

app.MapGet("/info", (IConfiguration configuration) =>
{
    return Results.Ok(new
    {
        Service = "Polyclinic API",
        Version = "1.0.0",
        Environment = app.Environment.EnvironmentName,
        Features = new
        {
            NATS = true,
            Database = "SQL Server",
            EntityFramework = "Enabled",
            Swagger = true
        },
        Configuration = new
        {
            NATS_Url = configuration["NATS:Url"],
            NATS_Subject = configuration["NATS:Subject"],
            Database = configuration.GetConnectionString("DefaultConnection")?.Split(';')[1]
        }
    });
});

try
{
    app.Logger.LogInformation("Starting Polyclinic Web API...");
    app.Logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
    
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    if (await dbContext.Database.CanConnectAsync())
    {
        app.Logger.LogInformation("Database connection successful");
        
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            app.Logger.LogInformation("Applying database migrations...");
            await dbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Migrations applied successfully");
        }
        else
        {
            app.Logger.LogInformation("Database is up to date");
        }
    }
    else
    {
        app.Logger.LogError("Cannot connect to database");
    }
    
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Application failed to start");
    throw;
}