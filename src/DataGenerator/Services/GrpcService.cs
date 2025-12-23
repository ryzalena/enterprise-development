using DataGenerator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DataGenerator.Services
{
    public interface IGrpcService
    {
        Task<bool> StreamContractsAsync(List<ContractMessage> contracts, CancellationToken cancellationToken);
    }

    public class GrpcService : IGrpcService
    {
        private readonly ILogger<GrpcService> _logger;
        private readonly GrpcConfig _config;

        public GrpcService(
            ILogger<GrpcService> logger,
            IOptions<GrpcConfig> config)
        {
            _logger = logger;
            _config = config.Value;
        }

        public Task<bool> StreamContractsAsync(List<ContractMessage> contracts, CancellationToken cancellationToken)
        {
            _logger.LogInformation("gRPC streaming is disabled. Would send {Count} contracts to {Url}", 
                contracts.Count, _config.ServerUrl);
            
            // Возвращаем true для имитации успешной отправки
            return Task.FromResult(true);
        }
    }
}