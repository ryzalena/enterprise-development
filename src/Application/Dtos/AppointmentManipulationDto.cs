namespace Application.Dtos;

/// <summary>
/// DTO для создания и обновления приема
/// </summary>
public class AppointmentManipulationDto
{
    /// <summary>
    /// Идентификатор пациента
    /// </summary>
    public int PatientId { get; set; }
    
    /// <summary>
    /// Идентификатор врача
    /// </summary>
    public int DoctorId { get; set; }
    
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