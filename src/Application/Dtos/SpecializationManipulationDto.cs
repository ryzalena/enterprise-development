namespace Application.Dtos;

/// <summary>
/// DTO для создания и обновления специализации врача
/// </summary>
public class SpecializationManipulationDto
{
    /// <summary>
    /// Название специализации
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Описание специализации
    /// </summary>
    public required string Description { get; set; }
}