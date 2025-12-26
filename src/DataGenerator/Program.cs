using DataGenerator.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.AddNatsClient("nats");

builder.Services.Configure<NatsConfig>(builder.Configuration.GetSection("NatsConfig"));
builder.Services.AddSingleton<INatsService, NatsService>();

// Логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Регистрация сервисов
builder.Services.AddSingleton<NatsService>();
builder.Services.AddHostedService<DataGenerationService>();

var host = builder.Build();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("🚀 Starting DataGenerator...");
    
    // Проверяем наличие NATS
    logger.LogInformation("Checking NATS connection...");
    
    // Получаем сервис NATS
    var natsService = host.Services.GetRequiredService<NatsService>();
    
    // Пытаемся подключиться (будет несколько попыток внутри метода)
    var connected = await natsService.ConnectWithRetryAsync();
    
    if (connected)
    {
        logger.LogInformation("✅ Successfully connected to NATS!");
        logger.LogInformation("📤 Will publish data to: polyclinic.patients");
        await host.RunAsync();
    }
    else
    {
        logger.LogError("❌ Failed to connect to NATS after all retries");
        logger.LogInformation("Will continue without NATS connection...");
        await host.RunAsync();
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "❌ DataGenerator failed to start");
    throw;
}