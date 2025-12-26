var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sqlserver")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("SA_PASSWORD", "YourStrong!Passw0rd")
    .AddDatabase("PolyclinicDB");

var nats = builder.AddNats("nats");

builder.AddProject<Projects.WebApi>("webapi")
    .WithReference(sql)
    .WithReference(nats);

builder.AddProject<Projects.DataGenerator>("datagenerator")
    .WithReference(sql)
    .WithReference(nats);

builder.Build().Run();