namespace Domain.Entities;

/// <summary>
/// Запись на прием к врачу
/// </summary>
public class Appointment
{
    /// <summary>
    /// Уникальный идентификатор записи
    /// </summary>
    public required int Id { get; set; }
    
    /// <summary>
    /// Идентификатор пациента
    /// </summary>
    public required int PatientId { get; set; }
    
    /// <summary>
    /// Пациент, записанный на прием
    /// </summary>
    public required Patient Patient { get; set; }
    
    /// <summary>
    /// Идентификатор врача
    /// </summary>
    public required int DoctorId { get; set; }
    
    /// <summary>
    /// Врач, к которому записан пациент
    /// </summary>
    public required Doctor Doctor { get; set; }
    
    /// <summary>
    /// Дата и время приема
    /// </summary>
    public required DateTime AppointmentDateTime { get; set; }
    
    /// <summary>
    /// Номер кабинета, в котором проходит прием
    /// </summary>
    public required string RoomNumber { get; set; }
    
    /// <summary>
    /// Признак того, является ли прием повторным
    /// </summary>
    public required bool IsFollowUp { get; set; }
}