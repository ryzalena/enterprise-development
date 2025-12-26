namespace DataGenerator.Services;

public interface INatsService
{
    Task ConnectAsync(CancellationToken cancellationToken);
    Task<bool> PublishContractAsync(AppointmentCreated appointment, CancellationToken cancellationToken);
    Task DisconnectAsync();
    bool IsConnected { get; }
}

public class NatsConfig
{
    public string Url { get; set; } = "nats://localhost:4222";
    public string Subject { get; set; } = "appointments.created";
    public int RetryCount { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 1000;
    public int TimeoutSeconds { get; set; } = 5;
}