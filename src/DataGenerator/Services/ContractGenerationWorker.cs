using DataGenerator.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DataGenerator.Services
{
    public class ContractGenerationWorker : BackgroundService
    {
        private readonly ILogger<ContractGenerationWorker> _logger;
        private readonly GeneratorConfig _config;
        private readonly IContractGenerator _contractGenerator;
        private readonly INatsService _natsService;
        private int _totalGenerated = 0;

        public ContractGenerationWorker(
            ILogger<ContractGenerationWorker> logger,
            IOptions<GeneratorConfig> config,
            IContractGenerator contractGenerator,
            INatsService natsService)
        {
            _logger = logger;
            _config = config.Value;
            _contractGenerator = contractGenerator;
            _natsService = natsService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting Contract Generator {GeneratorId}", _config.Id);
            _logger.LogInformation("Configuration: BatchSize={BatchSize}, Interval={Interval}ms, MaxContracts={Max}", 
                _config.BatchSize, _config.GenerationIntervalMs, _config.MaxContracts);

            // Подключаемся к NATS
            try
            {
                await _natsService.ConnectAsync(stoppingToken);
                _logger.LogInformation("Connected to NATS successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to NATS. Will continue without NATS.");
            }

            // Основной цикл генерации
            while (!stoppingToken.IsCancellationRequested && 
                   (_config.MaxContracts <= 0 || _totalGenerated < _config.MaxContracts))
            {
                try
                {
                    // Генерируем контракты
                    var contracts = _contractGenerator.GenerateContracts(_config.BatchSize, _config.Id);
                    _totalGenerated += contracts.Count;
                    
                    _logger.LogInformation("Generated {Count} contracts, total: {Total}", 
                        contracts.Count, _totalGenerated);

                    // Отправляем через NATS
                    var natsTasks = new List<Task<bool>>();
                    foreach (var contract in contracts)
                    {
                        var task = _natsService.PublishContractAsync(contract, stoppingToken);
                        natsTasks.Add(task);
                    }

                    // Ждем завершения отправки
                    var results = await Task.WhenAll(natsTasks);
                    var successCount = results.Count(r => r);
                    
                    _logger.LogInformation("Successfully sent {Success}/{Total} contracts via NATS", 
                        successCount, contracts.Count);

                    // Ожидание перед следующей генерацией
                    if (_config.GenerationIntervalMs > 0)
                    {
                        await Task.Delay(_config.GenerationIntervalMs, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during contract generation/sending");
                    await Task.Delay(5000, stoppingToken); // Ждем перед повторной попыткой
                }
            }

            _logger.LogInformation("Contract generation completed. Total generated: {Total}", _totalGenerated);
            
            // Отключаемся от NATS
            await _natsService.DisconnectAsync();
        }
    }
}