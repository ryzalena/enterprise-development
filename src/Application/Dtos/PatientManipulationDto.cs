namespace Application.Dtos;

/// <summary>
/// DTO для создания и обновления информации о пациенте
/// </summary>
public class PatientManipulationDto
{
    /// <summary>
    /// Номер паспорта пациента
    /// </summary>
    public required string PassportNumber { get; set; }
    
    /// <summary>
    /// Полное имя пациента
    /// </summary>
    public required string FullName { get; set; }
    
    /// <summary>
    /// Пол пациента
    /// </summary>
    public required string Gender { get; set; }
    
    /// <summary>
    /// Дата рождения пациента
    /// </summary>
    public required DateOnly BirthDate { get; set; }
    
    /// <summary>
    /// Адрес проживания пациента
    /// </summary>
    public required string Address { get; set; }
    
    /// <summary>
    /// Группа крови пациента
    /// </summary>
    public required string BloodGroup { get; set; }
    
    /// <summary>
    /// Резус-фактор пациента
    /// </summary>
    public required string RhFactor { get; set; }
    
    /// <summary>
    /// Номер телефона пациента
    /// </summary>
    public required string PhoneNumber { get; set; }
}