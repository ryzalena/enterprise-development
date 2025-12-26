using Bogus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DataGenerator.Services;

public class DataGenerationService : BackgroundService
{
    private readonly NatsService _natsService;
    private readonly ILogger<DataGenerationService> _logger;
    private readonly Faker _faker = new("ru");

    public DataGenerationService(
        NatsService natsService, 
        ILogger<DataGenerationService> logger)
    {
        _natsService = natsService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("📊 Data generation service started");
        
        // Ждем 5 секунд чтобы убедиться что NATS подключился
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        
        var patientCount = 0;
        var doctorCount = 0;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Чередуем генерацию пациентов и врачей
                if (patientCount <= doctorCount)
                {
                    // Генерация пациента
                    patientCount++;
                    var patient = new
                    {
                        Type = "Patient",
                        Id = Guid.NewGuid(),
                        PatientId = patientCount,
                        FirstName = _faker.Name.FirstName(),
                        LastName = _faker.Name.LastName(),
                        BirthDate = _faker.Date.Past(70, DateTime.Now.AddYears(-18)),
                        InsuranceNumber = $"INS-{DateTime.Now:yyyyMMdd}-{patientCount:0000}",
                        Phone = _faker.Phone.PhoneNumber(),
                        GeneratedAt = DateTime.UtcNow
                    };
                    
                    await _natsService.PublishAsync(patient);
                    _logger.LogInformation($"👤 Patient #{patientCount}: {patient.LastName} {patient.FirstName}");
                }
                else
                {
                    // Генерация врача
                    doctorCount++;
                    var doctor = new
                    {
                        Type = "Doctor",
                        Id = Guid.NewGuid(),
                        DoctorId = doctorCount,
                        FirstName = _faker.Name.FirstName(),
                        LastName = _faker.Name.LastName(),
                        Specialization = _faker.PickRandom(
                            new[] { "Терапевт", "Хирург", "Кардиолог", "Невролог", "Педиатр" }
                        ),
                        License = $"LIC-{DateTime.Now.Year}-{doctorCount:0000}",
                        GeneratedAt = DateTime.UtcNow
                    };
                    
                    await _natsService.PublishAsync(doctor);
                    _logger.LogInformation($"👨‍⚕️ Doctor #{doctorCount}: {doctor.LastName} {doctor.FirstName} ({doctor.Specialization})");
                }
                
                // Случайная задержка 3-8 секунд
                var delay = _faker.Random.Int(3000, 8000);
                await Task.Delay(delay, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in data generation");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}