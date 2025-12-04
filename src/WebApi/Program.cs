using System.Reflection;
using Application.Services;
using Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Polyclinic API",
        Version = "v1",
        Description = "API for Polyclinic Management System"
    });

    // Добавляем XML-комментарии из основного проекта
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Если DTO находятся в отдельном проекте Application, добавляем и его XML
    try
    {
        var dtoAssembly = typeof(Application.Dtos.AppointmentDto).Assembly;
        var dtoXmlFile = $"{dtoAssembly.GetName().Name}.xml";
        var dtoXmlPath = Path.Combine(AppContext.BaseDirectory, dtoXmlFile);
        
        if (File.Exists(dtoXmlPath))
        {
            options.IncludeXmlComments(dtoXmlPath);
        }
    }
    catch
    {
        // Если не удалось найти XML для DTO, просто продолжаем
    }
});

// Регистрация сервисов
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<ISpecializationService, SpecializationService>();

// Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Middleware pipeline
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Polyclinic API v1");
    options.RoutePrefix = "swagger";
    options.DisplayRequestDuration(); 
});

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Polyclinic WebAPI is running! Visit /swagger for API documentation.");

app.Run();