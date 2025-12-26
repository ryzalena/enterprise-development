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
                       ?? "Server=.\\SQLEXPRESS;Database=PolyclinicDB;Trusted_Connection=True;TrustServerCertificate=True;";

// Логируем (для отладки, скрывая чувствительные данные)
if (builder.Environment.IsDevelopment())
{
    var safeConnectionString = connectionString.Contains("Password=") 
        ? connectionString.Replace("Password=", "Password=***") 
        : connectionString;
    Console.WriteLine($"🔧 Using connection: {safeConnectionString}");
}

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

// Swagger с XML комментариями
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Polyclinic API", 
        Version = "v1",
        Description = "API для управления поликлиникой: пациенты, врачи, записи на приём",
        Contact = new OpenApiContact
        {
            Name = "Разработчик",
            Email = "dev@example.com"
        }
    });
    
    // Включение XML-комментариев
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
        Console.WriteLine($"✅ XML комментарии загружены из: {xmlPath}");
    }
    else
    {
        Console.WriteLine($"⚠️ XML файл не найден: {xmlPath}");
        Console.WriteLine("ℹ️ Убедитесь, что в проекте включена генерация XML документации");
        Console.WriteLine("ℹ️ Добавьте в WebApi.csproj: <GenerateDocumentationFile>true</GenerateDocumentationFile>");
    }
    
    // Опционально: включение аннотаций
    // c.EnableAnnotations();
    
    // Настройка безопасности (если используется)
    // c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    // {
    //     Description = "JWT Authorization header using the Bearer scheme.",
    //     Name = "Authorization",
    //     In = ParameterLocation.Header,
    //     Type = SecuritySchemeType.Http,
    //     Scheme = "bearer"
    // });
    
    // c.AddSecurityRequirement(new OpenApiSecurityRequirement
    // {
    //     {
    //         new OpenApiSecurityScheme
    //         {
    //             Reference = new OpenApiReference
    //             {
    //                 Type = ReferenceType.SecurityScheme,
    //                 Id = "Bearer"
    //             }
    //         },
    //         Array.Empty<string>()
    //     }
    // });
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
        options.DefaultModelsExpandDepth(2);
        options.DefaultModelExpandDepth(2);
        options.DisplayOperationId();
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        options.EnableDeepLinking();
        options.EnableFilter();
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
    
    app.Logger.LogInformation("API is running at: https://localhost:5000");
    app.Logger.LogInformation("Swagger: https://localhost:5000/swagger");
    app.Logger.LogInformation("Health check: https://localhost:5000/health");
    
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Application failed to start");
    
    // Детальная информация об ошибке
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nFATAL ERROR: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"   Inner Exception: {ex.InnerException.Message}");
    }
    Console.ResetColor();
    
    throw;
}