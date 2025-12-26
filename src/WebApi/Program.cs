using Infrastructure.Data;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Services;
using NATS.Client.Core; // Добавьте для NATS

var builder = WebApplication.CreateBuilder(args);

// Конфигурация
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var services = builder.Services;

// Контроллеры
services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=localhost,1433;Database=PolyclinicDB;User Id=sa;Password=MySecurePassword123!;TrustServerCertificate=True;";

services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});

// Репозитории
services.AddScoped<IPatientRepository, PatientRepository>();
services.AddScoped<IDoctorRepository, DoctorRepository>();
services.AddScoped<ISpecializationRepository, SpecializationRepository>();
services.AddScoped<IAppointmentRepository, AppointmentRepository>();

// Сервисы
services.AddScoped<IPatientService, PatientService>();
services.AddScoped<IDoctorService, DoctorService>();
services.AddScoped<IAppointmentService, AppointmentService>();
services.AddScoped<ISpecializationService, SpecializationService>();

// NATS подключение (опционально, если нужно в WebApi)
var natsUrl = builder.Configuration["NatsConfig:Url"] ?? "nats://localhost:4222";
services.AddSingleton<NatsConnection>(_ => 
{
    var opts = new NatsOpts
    {
        Url = natsUrl,
        Name = "WebApi",
        ConnectTimeout = TimeSpan.FromSeconds(10)
    };
    return new NatsConnection(opts);
});

// CORS
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
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Polyclinic API", 
        Version = "v1" 
    });
});

var app = builder.Build();

// Добавьте обработку ошибок для Development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Polyclinic API v1");
        options.RoutePrefix = "swagger";
        options.EnableTryItOutByDefault();
        options.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

// Маршруты
app.MapControllers();
app.MapGet("/", () => "Polyclinic API is running. Go to /swagger for documentation.");

// Простой health check
app.MapGet("/health", () => new 
{ 
    Status = "Healthy", 
    Timestamp = DateTime.UtcNow,
    Service = "Polyclinic API"
});

// Запуск
try
{
    app.Logger.LogInformation("Starting Polyclinic API...");
    
    // Проверка базы данных
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    if (await dbContext.Database.CanConnectAsync())
    {
        app.Logger.LogInformation("✅ Database connected successfully");
    }
    else
    {
        app.Logger.LogWarning("⚠️ Cannot connect to database");
    }
    
    app.Logger.LogInformation("🌐 API is running at: https://localhost:5000");
    app.Logger.LogInformation("📚 Swagger: https://localhost:5000/swagger");
    app.Logger.LogInformation("🏥 Health check: https://localhost:5000/health");
    
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "❌ Application failed to start");
    
    // Детальная информация об ошибке
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n❌ FATAL ERROR: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"   Inner Exception: {ex.InnerException.Message}");
    }
    Console.ResetColor();
    
    throw;
}