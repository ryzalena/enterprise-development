using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sqlserver")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("SA_PASSWORD", "YourStrong!Passw0rd")
    .AddDatabase("clinicdb");

var nats = builder.AddContainer("nats", "nats")
    .WithArgs("--jetstream");

var api = builder.AddProject<Projects.WebApi>("webapi")
    .WithReference(sql)
    .WithEnvironment("NATS__Url", "nats://nats:4222")
    .WithEnvironment("NATS__Subject", "appointments.created");

var generator = builder.AddProject<Projects.DataGenerator>("datagenerator")
    .WithEnvironment("NATS__Url", "nats://nats:4222")
    .WithEnvironment("NATS__Subject", "appointments.created");

builder.Build().Run();