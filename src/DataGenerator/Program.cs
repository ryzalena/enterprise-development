using DataGenerator.Models;
using DataGenerator.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        
        // Регистрация конфигураций
        services.Configure<GeneratorConfig>(configuration.GetSection("Generator"));
        services.Configure<GrpcConfig>(configuration.GetSection("Grpc"));
        services.Configure<NatsConfig>(configuration.GetSection("Nats"));
        
        // Регистрация сервисов
        services.AddSingleton<IContractGenerator, ContractGenerator>();
        services.AddSingleton<INatsService, NatsService>();
        services.AddSingleton<IGrpcService, GrpcService>();
        services.AddHostedService<ContractGenerationWorker>();
        
        // Настройка логирования
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
    })
    .Build();

await host.RunAsync();