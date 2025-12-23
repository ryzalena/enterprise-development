using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities;

namespace WebApi.Models.Domain;

[Table("MedicalContracts")]
public class MedicalContract
{
    [Key]
    [StringLength(50)]
    public string Id { get; set; } = null!;
    
    [Required]
    [StringLength(20)]
    public string PatientId { get; set; } = null!;
    
    [Required]
    [StringLength(20)]
    public string DoctorId { get; set; } = null!;
    
    [Required]
    [StringLength(100)]
    public string ServiceType { get; set; } = null!;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = null!;
    
    public DateTime CreatedDate { get; set; }
    public DateTime ValidUntil { get; set; }
    
    [StringLength(20)]
    public string? AppointmentId { get; set; }
    
    [StringLength(500)]
    public string? Diagnosis { get; set; }
    
    // JSON для хранения списка лекарств
    public string? PrescribedMedicationsJson { get; set; }
    
    public string? TreatmentPlan { get; set; }
    
    // Метаданные
    [StringLength(50)]
    public string? GeneratorId { get; set; }
    
    public DateTime ReceivedAt { get; set; }
    
    [StringLength(20)]
    public string Source { get; set; } = "gRPC";
    
    // Навигационные свойства (если есть связи в вашей модели)
    public virtual Patient? Patient { get; set; }
    public virtual Doctor? Doctor { get; set; }
    public virtual Appointment? Appointment { get; set; }
    
    // Метод для получения списка лекарств
    public List<string> GetPrescribedMedications()
    {
        if (string.IsNullOrEmpty(PrescribedMedicationsJson))
            return new List<string>();
            
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(PrescribedMedicationsJson) 
                ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }
    
    // Метод для установки списка лекарств
    public void SetPrescribedMedications(List<string> medications)
    {
        PrescribedMedicationsJson = System.Text.Json.JsonSerializer.Serialize(medications);
    }
}