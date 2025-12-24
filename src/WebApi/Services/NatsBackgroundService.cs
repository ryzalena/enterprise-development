using System.Text.Json;
using NATS.Client.Core;

namespace WebApi.Services;

public class NatsBackgroundService : BackgroundService
{
    private readonly ILogger<NatsBackgroundService> _logger;
    private readonly string _natsUrl;
    private readonly string _subject;
    
    public NatsBackgroundService(
        ILogger<NatsBackgroundService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _natsUrl = configuration["NATS:Url"] ?? "nats://localhost:4222";
        _subject = configuration["NATS:Subject"] ?? "appointments.created";
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting NATS background service...");
        
        var opts = NatsOpts.Default with
        {
            Url = _natsUrl,
            Name = $"API-{Guid.NewGuid():N}"
        };
        
        await using var natsConnection = new NatsConnection(opts);
        
        try
        {
            await natsConnection.ConnectAsync();
            _logger.LogInformation("Connected to NATS at {Url}", _natsUrl);
            
            await foreach (var msg in natsConnection.SubscribeAsync<string>(_subject, cancellationToken: stoppingToken))
            {
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var appointment = JsonSerializer.Deserialize<AppointmentCreated>(msg.Data, options);
                    
                    if (appointment != null)
                    {
                        _logger.LogInformation(
                            "Received appointment: Patient={PatientId}, Doctor={DoctorId}, Date={Date}", 
                            appointment.PatientId, appointment.DoctorId, appointment.AppointmentDate);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to NATS");
        }
    }
}