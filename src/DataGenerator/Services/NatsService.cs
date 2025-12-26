using NATS.Client.Core;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DataGenerator.Services;

public class NatsService : INatsService, IAsyncDisposable
{
    private readonly ILogger<NatsService> _logger;
    private readonly NatsConfig _config;
    private NatsConnection? _connection;
    private bool _isConnected = false;
    
    public bool IsConnected => _isConnected;
    
    public NatsService(ILogger<NatsService> logger, IOptions<NatsConfig> config)
    {
        _logger = logger;
        _config = config.Value;
    }

    // Реализация интерфейса
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        await ConnectWithRetryAsync(_config.RetryCount, _config.RetryDelayMs, cancellationToken);
    }

    // Сделайте метод public или internal (в зависимости от того, где вы его вызываете)
    public async Task<bool> ConnectWithRetryAsync(
        int maxRetries = 10, 
        int delayMs = 3000, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Trying to connect to NATS (max {maxRetries} retries)...");
        
        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                _logger.LogInformation($"Attempt {i+1}/{maxRetries}: Connecting to {_config.Url}");
                
                var options = new NatsOpts
                {
                    Url = _config.Url,
                    Name = "DataGenerator",
                    ConnectTimeout = TimeSpan.FromSeconds(_config.TimeoutSeconds),
                    ReconnectWaitMax = TimeSpan.FromSeconds(5)
                };
                
                _connection = new NatsConnection(options);
                
                // Проверка соединения
                await _connection.PingAsync(cancellationToken);
                _isConnected = true;
                
                _logger.LogInformation("✅ NATS connection successful!");
                _logger.LogInformation($"Server: {_config.Url}");
                _logger.LogInformation($"Client: DataGenerator");
                _logger.LogInformation($"Subject: {_config.Subject}");
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Attempt {i+1} failed: {ex.Message}");
                
                if (i < maxRetries - 1)
                {
                    _logger.LogInformation($"Waiting {delayMs}ms before next attempt...");
                    await Task.Delay(delayMs, cancellationToken);
                }
            }
        }
        
        _logger.LogError($"❌ Failed to connect to NATS after {maxRetries} attempts");
        _logger.LogInformation("Make sure NATS is running:");
        _logger.LogInformation("1. docker run -d -p 4222:4222 nats:latest");
        _logger.LogInformation("2. Or via .NET Aspire AppHost");
        
        return false;
    }

    // Реализация интерфейса
    public async Task<bool> PublishContractAsync(AppointmentCreated appointment, CancellationToken cancellationToken)
    {
        if (!_isConnected || _connection == null)
        {
            _logger.LogWarning("Not connected to NATS, trying to reconnect...");
            await ConnectAsync(cancellationToken);
        }
        
        try
        {
            var json = JsonSerializer.Serialize(appointment);
            await _connection!.PublishAsync(_config.Subject, json, cancellationToken: cancellationToken);
            _logger.LogDebug($"Published appointment to {_config.Subject}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish appointment to NATS");
            _isConnected = false;
            return false;
        }
    }

    // Дополнительный метод для публикации любых данных
    public async Task PublishAsync<T>(T data, string? customSubject = null, CancellationToken cancellationToken = default)
    {
        var subject = customSubject ?? _config.Subject;
        
        if (!_isConnected || _connection == null)
        {
            _logger.LogWarning("Not connected to NATS, trying to reconnect...");
            await ConnectAsync(cancellationToken);
        }
        
        try
        {
            var json = JsonSerializer.Serialize(data);
            await _connection!.PublishAsync(subject, json, cancellationToken: cancellationToken);
            _logger.LogDebug($"Published to {subject}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to publish to {subject}");
            _isConnected = false;
        }
    }

    public async Task DisconnectAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync();
            _connection = null;
            _isConnected = false;
            _logger.LogInformation("Disconnected from NATS");
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }
}