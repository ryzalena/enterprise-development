using DataGenerator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client.Core;
using Polly;
using System.Text.Json;

namespace DataGenerator.Services
{
    public interface INatsService
    {
        Task ConnectAsync(CancellationToken cancellationToken);
        Task<bool> PublishContractAsync(ContractMessage contract, CancellationToken cancellationToken);
        Task DisconnectAsync();
        bool IsConnected { get; }
    }

    public class NatsService : INatsService, IAsyncDisposable
    {
        private readonly NatsConfig _config;
        private readonly ILogger<NatsService> _logger;
        private readonly ResiliencePipeline _resiliencePipeline;
        private INatsConnection? _connection;

        // Исправлено: проверка состояния соединения
        public bool IsConnected => _connection != null;

        public NatsService(
            IOptions<NatsConfig> config,
            ILogger<NatsService> logger)
        {
            _config = config.Value;
            _logger = logger;
            
            _resiliencePipeline = new ResiliencePipelineBuilder()
                .AddRetry(new Polly.Retry.RetryStrategyOptions
                {
                    MaxRetryAttempts = _config.RetryCount,
                    Delay = TimeSpan.FromMilliseconds(_config.RetryDelayMs),
                    BackoffType = DelayBackoffType.Exponential,
                    OnRetry = args =>
                    {
                        _logger.LogWarning("Retry attempt {Attempt} for NATS. Delay: {Delay}ms", 
                            args.AttemptNumber, args.RetryDelay.TotalMilliseconds);
                        return default;
                    }
                })
                .Build();
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            if (IsConnected)
                return;

            await _resiliencePipeline.ExecuteAsync(async token =>
            {
                try
                {
                    _logger.LogInformation("Connecting to NATS at {Url}", _config.Url);
                    
                    var options = NatsOpts.Default with
                    {
                        Url = _config.Url,
                        Name = $"Generator-{Guid.NewGuid():N}",
                        ConnectTimeout = TimeSpan.FromSeconds(_config.TimeoutSeconds)
                    };

                    _connection = new NatsConnection(options);
                    
                    // Простая проверка подключения - отправка ping
                    await _connection.PingAsync(token);
                    
                    _logger.LogInformation("Connected to NATS successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to connect to NATS");
                    throw;
                }
            }, cancellationToken);
        }

        public async Task<bool> PublishContractAsync(ContractMessage contract, CancellationToken cancellationToken)
        {
            if (!IsConnected || _connection == null)
            {
                _logger.LogWarning("NATS not connected. Attempting to reconnect...");
                await ConnectAsync(cancellationToken);
            }

            return await _resiliencePipeline.ExecuteAsync(async token =>
            {
                try
                {
                    var json = JsonSerializer.Serialize(contract);
                    await _connection!.PublishAsync(_config.Subject, json, cancellationToken: token);
                    
                    _logger.LogDebug("Published contract {ContractId} to NATS", contract.Id);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish contract {ContractId}", contract.Id);
                    return false;
                }
            }, cancellationToken);
        }

        public async Task DisconnectAsync()
        {
            if (_connection != null)
            {
                await _connection.DisposeAsync();
                _connection = null;
                _logger.LogInformation("Disconnected from NATS");
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync();
        }
    }
}