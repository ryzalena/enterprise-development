namespace Domain.Entities;

/// <summary>
/// Врач поликлиники
/// </summary>
public class Doctor
{
    /// <summary>
    /// Номер паспорта врача (уникальный идентификатор)
    /// </summary>
    public required string PassportNumber { get; set; }
    
    /// <summary>
    /// ФИО врача
    /// </summary>
    public required string FullName { get; set; }
    
    /// <summary>
    /// Год рождения врача
    /// </summary>
    public required int BirthYear { get; set; }
    
    /// <summary>
    /// Специализация врача
    /// </summary>
    public required Specialization Specialization { get; set; }
    
    /// <summary>
    /// Стаж работы врача (в годах)
    /// </summary>
    public required int ExperienceYears { get; set; }
}