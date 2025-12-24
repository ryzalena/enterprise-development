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
        services.Configure<NatsConfig>(context.Configuration.GetSection("NATS"));
        
        services.AddSingleton<INatsService, NatsService>();
        services.AddHostedService<ContractGeneratorService>();
        
        services.AddLogging(configure => 
        {
            configure.ClearProviders();
            configure.AddConsole();
            configure.AddDebug();
            configure.SetMinimumLevel(LogLevel.Information);
        });
    })
    .Build();

await host.RunAsync();