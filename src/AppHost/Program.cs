using Aspire.Hosting;
using System;

// Устанавливаем переменные
Environment.SetEnvironmentVariable("ASPIRE_DASHBOARD_PATH", @"C:\Users\456\.dotnet\tools\dashboard");
Environment.SetEnvironmentVariable("ASPIRE_DCP_PATH", @"C:\Users\456\.dotnet\tools\dcp.exe");
Environment.SetEnvironmentVariable("ASPIRE_ALLOW_DCP_FALLBACK", "true");

Console.WriteLine("=== ASPIRE ===");
Console.WriteLine("Dashboard: " + Environment.GetEnvironmentVariable("ASPIRE_DASHBOARD_PATH"));
Console.WriteLine("DCP: " + Environment.GetEnvironmentVariable("ASPIRE_DCP_PATH"));
Console.WriteLine("==============");

var builder = DistributedApplication.CreateBuilder(args);

// SQL Server
var sql = builder.AddSqlServer("sqlserver")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("SA_PASSWORD", "YourStrong!Passw0rd")
    .AddDatabase("clinicdb");

// WebApi
var api = builder.AddProject("webapi", @"..\WebApi\WebApi.csproj")
    .WithReference(sql);

builder.Build().Run();