using Infrastructure.Data;
using Domain.TestData;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ========== КОНФИГУРАЦИЯ СЕРВИСОВ ==========
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext - КРИТИЧЕСКОЕ ИСПРАВЛЕНИЕ ДЛЯ ASPIRE
// Aspire передает строку подключения как "clinicdb"
var connectionString = builder.Configuration.GetConnectionString("clinicdb");

if (string.IsNullOrEmpty(connectionString))
{
    // Для отладки: проверяем все возможные варианты
    Console.WriteLine("=== ASPIRE CONFIGURATION DEBUG ===");
    Console.WriteLine("Connection string 'clinicdb' not found.");
    
    // Проверяем все ключи конфигурации
    var allKeys = builder.Configuration.AsEnumerable()
        .Where(kv => kv.Key.Contains("Connection", StringComparison.OrdinalIgnoreCase))
        .ToList();
    
    if (allKeys.Any())
    {
        Console.WriteLine("Found related configuration keys:");
        foreach (var kv in allKeys)
        {
            Console.WriteLine($"  {kv.Key} = {kv.Value}");
        }
    }
    else
    {
        Console.WriteLine("No connection-related configuration found.");
    }
    
    // Fallback для разработки - прямое подключение к контейнеру Aspire
    connectionString = "Server=sqlserver;Database=clinicdb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true;";
    Console.WriteLine($"Using fallback connection to Aspire SQL container");
}
else
{
    Console.WriteLine($"Aspire connection string received ({connectionString.Length} chars)");
    // Для безопасности не выводим полную строку
    if (connectionString.Length > 30)
    {
        Console.WriteLine($"Connection preview: {connectionString.Substring(0, 30)}...");
    }
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// ========== КОНФИГУРАЦИЯ PIPELINE ==========
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// ========== ASPIRE HEALTH CHECK ENDPOINT ==========
app.MapGet("/api/health", (IConfiguration config) => Results.Ok(new 
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Message = "Clinic API is running with .NET Aspire",
    AspireConfigured = !string.IsNullOrEmpty(config.GetConnectionString("clinicdb")),
    Environment = app.Environment.EnvironmentName,
    ConnectionSources = new
    {
        ClinicDb = !string.IsNullOrEmpty(config.GetConnectionString("clinicdb")),
        DefaultConnection = !string.IsNullOrEmpty(config.GetConnectionString("DefaultConnection"))
    }
}));

// ========== DEBUG ENDPOINT FOR ASPIRE CONFIGURATION ==========
app.MapGet("/api/debug/aspire-config", (IConfiguration config) =>
{
    var result = new
    {
        Timestamp = DateTime.UtcNow,
        EnvironmentVariables = Environment.GetEnvironmentVariables()
            .Cast<System.Collections.DictionaryEntry>()
            .Where(e => e.Key.ToString()?.Contains("CONNECTION", StringComparison.OrdinalIgnoreCase) == true)
            .ToDictionary(
                e => e.Key.ToString()!, 
                e => e.Value?.ToString()?.Length > 50 
                    ? e.Value.ToString()?.Substring(0, 50) + "..." 
                    : e.Value?.ToString()
            ),
        Configuration = config.AsEnumerable()
            .Where(kv => kv.Key.Contains("Connection", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(
                kv => kv.Key,
                kv => kv.Value?.Length > 50 
                    ? kv.Value.Substring(0, 50) + "..." 
                    : kv.Value
            )
    };
    
    return Results.Ok(result);
});

// ========== ВАШИ СУЩЕСТВУЮЩИЕ ENDPOINTS (без изменений) ==========
app.MapGet("/api/test-data/info", async (ApplicationDbContext context) =>
{
    var data = new
    {
        Patients = await context.Patients
            .Select(p => new { p.Id, p.FullName })
            .OrderBy(p => p.Id)
            .ToListAsync(),
        Doctors = await context.Doctors
            .Select(d => new { d.Id, d.FullName, d.SpecializationId })
            .OrderBy(d => d.Id)
            .ToListAsync(),
        Specializations = await context.Specializations
            .Select(s => new { s.Id, s.Name })
            .OrderBy(s => s.Id)
            .ToListAsync(),
        Appointments = await context.Appointments
            .Select(a => new { a.Id, a.PatientId, a.DoctorId, a.AppointmentDateTime })
            .OrderBy(a => a.Id)
            .ToListAsync()
    };
    
    return Results.Ok(data);
});

app.MapPost("/api/seed/force-reseed", async (HttpContext httpContext) =>
{
    try
    {
        var context = httpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
        var logger = httpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        
        await ForceSeedDatabaseAsync(context, logger);
        
        return Results.Ok(new
        {
            Success = true,
            Message = "Database force-reseeded successfully!",
            Timestamp = DateTime.UtcNow
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Force reseed failed: {ex.Message}");
    }
});

// ========== МИГРАЦИИ И СИДИНГ ==========
await ApplyMigrations(app);

app.Run();

// ========== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ==========
async Task ApplyMigrations(WebApplication webApp)
{
    using var scope = webApp.Services.CreateScope();
    var services = scope.ServiceProvider;
    
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("=== ASPIRE: APPLYING DATABASE MIGRATIONS ===");
        await context.Database.MigrateAsync();
        
        logger.LogInformation("=== ASPIRE: SEEDING DATABASE ===");
        await ForceSeedDatabaseAsync(context, logger);
        
        logger.LogInformation("=== ASPIRE: DATABASE SETUP COMPLETED ===");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while setting up the database.");
        throw;
    }
}

async Task ForceSeedDatabaseAsync(ApplicationDbContext context, ILogger logger)
{
    logger.LogInformation("=== STARTING FORCE DATABASE SEED ===");
    
    try
    {
        // 1. Очистка данных
        logger.LogInformation("Step 1: Clearing all existing data...");
        
        context.Appointments.RemoveRange(await context.Appointments.ToListAsync());
        context.Doctors.RemoveRange(await context.Doctors.ToListAsync());
        context.Patients.RemoveRange(await context.Patients.ToListAsync());
        context.Specializations.RemoveRange(await context.Specializations.ToListAsync());
        
        await context.SaveChangesAsync();
        logger.LogInformation("✓ All tables cleared");
        
        // 2. Создание специализаций
        logger.LogInformation("Step 2: Seeding specializations...");
        
        var specializations = new List<Specialization>();
        for (var i = 0; i < TestData.Specializations.Count; i++)
        {
            var spec = TestData.Specializations[i];
            specializations.Add(new Specialization 
            { 
                Name = spec.Name, 
                Description = spec.Description 
            });
        }
        
        await context.Specializations.AddRangeAsync(specializations);
        await context.SaveChangesAsync();
        
        logger.LogInformation($"✓ Seeded {specializations.Count} specializations");
        
        // 3. Создание пациентов
        logger.LogInformation("Step 3: Seeding patients...");
        
        var patients = new List<Patient>();
        for (var i = 0; i < TestData.Patients.Count; i++)
        {
            var patient = TestData.Patients[i];
            patients.Add(new Patient
            {
                PassportNumber = patient.PassportNumber,
                FullName = patient.FullName,
                Gender = patient.Gender,
                BirthDate = patient.BirthDate,
                Address = patient.Address,
                BloodGroup = patient.BloodGroup,
                RhFactor = patient.RhFactor,
                PhoneNumber = patient.PhoneNumber
            });
        }
        
        await context.Patients.AddRangeAsync(patients);
        await context.SaveChangesAsync();
        
        logger.LogInformation($"✓ Seeded {patients.Count} patients");
        
        // 4. Создание врачей
        logger.LogInformation("Step 4: Seeding doctors...");
        
        var doctors = new List<Doctor>();
        for (var i = 0; i < Math.Min(TestData.Doctors.Count, 10); i++)
        {
            var doctor = TestData.Doctors[i];
            doctors.Add(new Doctor
            {
                PassportNumber = doctor.PassportNumber,
                FullName = doctor.FullName,
                BirthYear = doctor.BirthYear,
                SpecializationId = specializations[i % specializations.Count].Id,
                ExperienceYears = doctor.ExperienceYears
            });
        }
        
        await context.Doctors.AddRangeAsync(doctors);
        await context.SaveChangesAsync();
        
        logger.LogInformation($"✓ Seeded {doctors.Count} doctors");
        
        // 5. Создание записей на прием
        logger.LogInformation("Step 5: Seeding appointments...");
        
        if (patients.Any() && doctors.Any())
        {
            var appointments = new List<Appointment>();
            var baseDate = DateTime.Now.Date.AddDays(1).AddHours(9);
            
            for (var i = 0; i < 10; i++)
            {
                var patient = patients[i % patients.Count];
                var doctor = doctors[i % doctors.Count];
                
                appointments.Add(new Appointment
                {
                    PatientId = patient.Id,
                    DoctorId = doctor.Id,
                    AppointmentDateTime = baseDate.AddDays(i / 2).AddHours((i % 2) * 4),
                    RoomNumber = $"10{i + 1:00}",
                    IsFollowUp = i % 3 == 0
                });
            }
            
            await context.Appointments.AddRangeAsync(appointments);
            await context.SaveChangesAsync();
            
            logger.LogInformation($"Seeded {appointments.Count} appointments");
        }
        else
        {
            logger.LogWarning("Cannot seed appointments: no patients or doctors available");
        }
        
        // 6. Финальная проверка
        logger.LogInformation("Step 6: Final verification...");
        
        var finalCounts = new
        {
            Patients = await context.Patients.CountAsync(),
            Doctors = await context.Doctors.CountAsync(),
            Specializations = await context.Specializations.CountAsync(),
            Appointments = await context.Appointments.CountAsync()
        };
        
        logger.LogInformation($"Patients: {finalCounts.Patients}");
        logger.LogInformation($"Doctors: {finalCounts.Doctors}");
        logger.LogInformation($"Specializations: {finalCounts.Specializations}");
        logger.LogInformation($"Appointments: {finalCounts.Appointments}");
        
        logger.LogInformation("=== FORCE SEED COMPLETED ===");
        
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Force seed failed!");
        Console.WriteLine($"ERROR: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
        }
        throw;
    }
}