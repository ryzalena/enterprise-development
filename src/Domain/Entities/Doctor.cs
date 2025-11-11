namespace Domain.Entities;

/// <summary>
/// Врач поликлиники
/// </summary>
public class Doctor
{
    /// <summary>
    /// Уникальный идентификатор врача
    /// </summary>
    public required int Id { get; set; }
    
    /// <summary>
    /// Номер паспорта врача
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