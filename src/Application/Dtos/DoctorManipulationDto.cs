namespace Application.Dtos;

public class DoctorManipulationDto
{
    /// <summary>
    /// Номер паспорта врача
    /// </summary>
    public required string PassportNumber { get; set; }
    
    /// <summary>
    /// Полное имя врача
    /// </summary>
    public required string FullName { get; set; }
    
    /// <summary>
    /// Год рождения врача
    /// </summary>
    public int BirthYear { get; set; }
    
    /// <summary>
    /// Идентификатор специализации врача
    /// </summary>
    public int SpecializationId { get; set; }
    
    /// <summary>
    /// Количество лет опыта работы
    /// </summary>
    public int ExperienceYears { get; set; }
}