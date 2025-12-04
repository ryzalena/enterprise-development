namespace Application.Dtos;

/// <summary>
/// DTO для представления информации о враче
/// </summary>
public class DoctorDto
{
    /// <summary>
    /// Уникальный идентификатор врача
    /// </summary>
    public int Id { get; set; }
    
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
    /// Название специализации врача
    /// </summary>
    public required string SpecializationName { get; set; }
    
    /// <summary>
    /// Количество лет опыта работы
    /// </summary>
    public int ExperienceYears { get; set; }
    
    /// <summary>
    /// Идентификатор специализации врача
    /// </summary>
    public int SpecializationId { get; set; }
}