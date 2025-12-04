namespace Application.Dtos;

/// <summary>
/// DTO для представления информации о специализации врача
/// </summary>
public class SpecializationDto
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
    /// Описание специализации (может отсутствовать)
    /// </summary>
    public string? Description { get; set; }
}