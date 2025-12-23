using Microsoft.EntityFrameworkCore;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Настройка Kestrel для gRPC
builder.WebHost.ConfigureKestrel(options =>
{
    // Порт для gRPC (HTTP/2)
    options.ListenAnyIP(5189, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
    
    // Порт для HTTP/1.1 (для Swagger и health checks)
    options.ListenAnyIP(5190, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
    });
});

// Регистрация сервисов
var services = builder.Services;

// Добавьте контроллеры
services.AddControllers();

// База данных
var connectionString = builder.Configuration.GetConnectionString("clinicdb") 
    ?? "Server=localhost,1433;Database=clinicdb;User Id=sa;Password=MySecurePassword123!;TrustServerCertificate=True;";

services.AddDbContext<DbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});

// Регистрация NATS сервиса
services.AddHostedService<SimpleNatsService>();

// Если у вас есть GrpcContractService - зарегистрируйте его
services.AddSingleton<GrpcContractService>();

// CORS
services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
    });
});

// Swagger
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

// Конфигурация middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRouting();

// Endpoints
app.MapControllers();

// Health checks
app.MapGet("/", () => "Polyclinic Web API is running.");
app.MapGet("/health", () => Results.Ok(new 
{ 
    Status = "Healthy", 
    Timestamp = DateTime.UtcNow 
}));

// Информация о сервисе
app.MapGet("/info", () =>
{
    return Results.Ok(new
    {
        Service = "Polyclinic Contract Service",
        Version = "1.0.0",
        Environment = app.Environment.EnvironmentName,
        Features = new
        {
            NATS = true,
            Database = "SQL Server"
        },
        Endpoints = new
        {
            REST = "http://localhost:5190",
            Health = "http://localhost:5190/health",
            API = "http://localhost:5190/api/contracts"
        }
    });
});

// Проверка подключения к БД при старте
try
{
    app.Logger.LogInformation("Starting Polyclinic Web API...");
    app.Logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
    app.Logger.LogInformation("REST endpoint: http://*:5190");
    
    // Проверяем подключение к БД
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
    
    if (await dbContext.Database.CanConnectAsync())
    {
        app.Logger.LogInformation("Database connection successful");
    }
    else
    {
        app.Logger.LogWarning("Cannot connect to database");
    }
    
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Application failed to start");
    throw;
}
