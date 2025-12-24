using Bogus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DataGenerator.Services;

public class ContractGeneratorService : BackgroundService
{
    private readonly INatsService _natsService;
    private readonly ILogger<ContractGeneratorService> _logger;
    private readonly Faker<AppointmentCreated> _appointmentFaker;
    
    public ContractGeneratorService(
        INatsService natsService,
        ILogger<ContractGeneratorService> logger)
    {
        _natsService = natsService;
        _logger = logger;
        
        _appointmentFaker = new Faker<AppointmentCreated>()
            .RuleFor(a => a.Id, f => Guid.NewGuid())
            .RuleFor(a => a.PatientId, f => f.Random.Number(1, 100))
            .RuleFor(a => a.DoctorId, f => f.Random.Number(1, 50))
            .RuleFor(a => a.AppointmentDate, f => f.Date.Future())
            .RuleFor(a => a.CreatedAt, f => DateTime.UtcNow);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _natsService.ConnectAsync(stoppingToken);
        
        _logger.LogInformation("Starting appointment generation...");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var appointment = _appointmentFaker.Generate();
                var success = await _natsService.PublishContractAsync(appointment, stoppingToken);
                
                if (success)
                {
                    _logger.LogInformation(
                        "Generated appointment {AppointmentId} for patient {PatientId} with doctor {DoctorId}", 
                        appointment.Id, appointment.PatientId, appointment.DoctorId);
                }
                else
                {
                    _logger.LogWarning("Failed to publish appointment {AppointmentId}", appointment.Id);
                }
                
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating appointment");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}