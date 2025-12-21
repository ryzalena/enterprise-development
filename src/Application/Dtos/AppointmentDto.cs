namespace Application.Dtos;

/// <summary>
/// DTO для представления информации о назначенном приеме
/// </summary>
public class AppointmentDto
{
    /// <summary>
    /// Уникальный идентификатор приема
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Идентификатор пациента
    /// </summary>
    public int PatientId { get; set; }
    
    /// <summary>
    /// Имя пациента
    /// </summary>
    public PatientDto? Patient { get; set; }
    
    /// <summary>
    /// Идентификатор врача
    /// </summary>
    public int DoctorId { get; set; }
    
    /// <summary>
    /// Имя врача
    /// </summary>
    public DoctorDto? Doctor { get; set; }
    
    /// <summary>
    /// Дата и время приема
    /// </summary>
    public DateTime AppointmentDateTime { get; set; }
    
    /// <summary>
    /// Номер кабинета
    /// </summary>
    public string? RoomNumber { get; set; }
    
    /// <summary>
    /// Признак повторного приема (follow-up)
    /// </summary>
    public bool IsFollowUp { get; set; }
}