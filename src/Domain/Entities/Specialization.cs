namespace Domain.Entities;

/// <summary>
/// Специализация врача
/// </summary>
public class Specialization
{
    /// <summary>
    /// Уникальный идентификатор специализации
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Название специализации
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Описание специализации (опционально)
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Врачи данной специализации
    /// </summary>
    public ICollection<Doctor> Doctors { get; set; } = [];
}