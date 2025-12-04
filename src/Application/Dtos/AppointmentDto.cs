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
    public string PatientName { get; set; } = string.Empty;
    
    /// <summary>
    /// Идентификатор врача
    /// </summary>
    public int DoctorId { get; set; }
    
    /// <summary>
    /// Имя врача
    /// </summary>
    public string DoctorName { get; set; } = string.Empty;
    
    /// <summary>
    /// Дата и время приема
    /// </summary>
    public DateTime AppointmentDateTime { get; set; }
    
    /// <summary>
    /// Номер кабинета
    /// </summary>
    public string RoomNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Признак повторного приема (follow-up)
    /// </summary>
    public bool IsFollowUp { get; set; }
}