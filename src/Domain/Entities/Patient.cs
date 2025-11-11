using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Пациент поликлиники
/// </summary>
public class Patient
{
    /// <summary>
    /// Уникальный идентификатор пациента
    /// </summary>
    public required int Id { get; set; }
    
    /// <summary>
    /// Номер паспорта пациента
    /// </summary>
    public required string PassportNumber { get; set; }
    
    /// <summary>
    /// ФИО пациента
    /// </summary>
    public required string FullName { get; set; }
    
    /// <summary>
    /// Пол пациента
    /// </summary>
    public required Gender Gender { get; set; }
    
    /// <summary>
    /// Дата рождения пациента
    /// </summary>
    public required DateTime BirthDate { get; set; }
    
    /// <summary>
    /// Адрес проживания пациента
    /// </summary>
    public required string Address { get; set; }
    
    /// <summary>
    /// Группа крови пациента
    /// </summary>
    public required BloodGroup BloodGroup { get; set; }
    
    /// <summary>
    /// Резус-фактор пациента
    /// </summary>
    public required RhFactor RhFactor { get; set; }
    
    /// <summary>
    /// Контактный телефон пациента
    /// </summary>
    public required string PhoneNumber { get; set; }

    /// <summary>
    /// Возраст пациента (вычисляемое свойство)
    /// </summary>
    public int Age => DateTime.Now.Year - BirthDate.Year;
}