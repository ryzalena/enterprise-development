using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Добавляем SQL Server
var sql = builder.AddSqlServer("sqlserver")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("SA_PASSWORD", "YourStrong!Passw0rd")
    .AddDatabase("clinicdb");

// Добавляем NATS как контейнер
var nats = builder.AddContainer("nats", "nats")
    .WithHttpEndpoint(8222, name: "monitoring")
    .WithArgs("--jetstream", "--trace", "--http_port=8222");

// Получаем endpoint для NATS
var natsEndpoint = nats.GetEndpoint("nats");
var natsMonitoringEndpoint = nats.GetEndpoint("monitoring");

// Добавляем WebApi
var api = builder.AddProject<Projects.WebApi>("webapi")
    .WithReference(sql)
    .WithHttpEndpoint(port: 5189, name: "http")
    .WithHttpsEndpoint(port: 7189, name: "https");

// Получаем endpoint для WebApi
var apiHttpEndpoint = api.GetEndpoint("http");

// Добавляем DataGenerator с правильной конфигурацией
var generator = builder.AddProject<Projects.DataGenerator>("datagenerator")
    // Используем явную строку для URL NATS
    .WithEnvironment("Nats__Url", $"nats://localhost:4222")
    .WithEnvironment("Nats__ContractSubject", "polyclinic.contracts.generated")
    // Используем endpoint WebApi
    .WithEnvironment("Grpc__ServerUrl", apiHttpEndpoint.Url);

// Настраиваем WebApi
api.WithEnvironment("Nats__Url", $"nats://localhost:4222")
    .WithEnvironment("Nats__ContractSubject", "polyclinic.contracts.generated");

// Логирование для отладки
Console.WriteLine("=== ASPIRE CONFIGURATION ===");
Console.WriteLine("WebApi HTTP URL: " + apiHttpEndpoint.Url);
Console.WriteLine("NATS URL: nats://localhost:4222");
Console.WriteLine("NATS Monitoring: http://localhost:8222");
Console.WriteLine("============================");

builder.Build().Run();