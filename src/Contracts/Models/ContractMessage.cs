using System.Text.Json.Serialization;

namespace Contracts.Models
{
    public class ContractMessage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [JsonPropertyName("patientId")]
        public string PatientId { get; set; }
        
        [JsonPropertyName("doctorId")]
        public string DoctorId { get; set; }
        
        [JsonPropertyName("serviceType")]
        public string ServiceType { get; set; }
        
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        
        [JsonPropertyName("status")]
        public string Status { get; set; }
        
        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        [JsonPropertyName("validUntil")]
        public DateTime ValidUntil { get; set; }
        
        [JsonPropertyName("appointmentId")]
        public string AppointmentId { get; set; }
        
        [JsonPropertyName("diagnosis")]
        public string Diagnosis { get; set; }
        
        [JsonPropertyName("prescribedMedications")]
        public List<string> PrescribedMedications { get; set; } = new();
        
        [JsonPropertyName("treatmentPlan")]
        public string TreatmentPlan { get; set; }
        
        [JsonPropertyName("generatorId")]
        public string GeneratorId { get; set; }
        
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class NatsContractMessage : ContractMessage
    {
        public string Subject { get; set; } = "contracts.generated";
        public string ReplyTo { get; set; }
    }
}