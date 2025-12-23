using Bogus;

namespace DataGenerator.Models
{
    public class GeneratorConfig
    {
        public string Id { get; set; } = "polyclinic-generator-01";
        public int BatchSize { get; set; } = 5;
        public int GenerationIntervalMs { get; set; } = 10000;
        public int MaxContracts { get; set; } = 50;
    }

    public class GrpcConfig
    {
        public string ServerUrl { get; set; } = "http://localhost:5189";
    }

    public class NatsConfig
    {
        public string Url { get; set; } = "nats://localhost:4222";
        public string Subject { get; set; } = "polyclinic.contracts.generated";
        public int RetryCount { get; set; } = 5;
        public int RetryDelayMs { get; set; } = 2000;
        public int MaxReconnectAttempts { get; set; } = 10;
        public int TimeoutSeconds { get; set; } = 30;
    }

    public class ContractMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PatientId { get; set; } = string.Empty;
        public string DoctorId { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ValidUntil { get; set; } = DateTime.UtcNow.AddDays(30);
        public string AppointmentId { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public List<string> PrescribedMedications { get; set; } = new();
        public string TreatmentPlan { get; set; } = string.Empty;
        public string GeneratorId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}