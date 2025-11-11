namespace Domain.Entities;

/// <summary>
/// Специализация врача
/// </summary>
public class Specialization
{
    /// <summary>
    /// Уникальный идентификатор специализации
    /// </summary>
    public required int Id { get; set; }
    
    /// <summary>
    /// Название специализации
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Описание специализации (опционально)
    /// </summary>
    public string? Description { get; set; }
}