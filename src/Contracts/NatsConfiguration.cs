namespace Contracts
{
    public class NatsConfiguration
    {
        public string Url { get; set; } = "nats://localhost:4222";
        public string Subject { get; set; } = "contracts.generated";
        public int RetryCount { get; set; } = 5;
        public int RetryDelayMs { get; set; } = 1000;
        public int MaxReconnectAttempts { get; set; } = 10;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}