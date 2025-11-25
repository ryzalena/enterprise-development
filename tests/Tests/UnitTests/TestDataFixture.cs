// TestDataFixture.cs
using Domain.Entities;
using Domain.TestData;

namespace Tests.UnitTests;

/// <summary>
/// Фикстура для предоставления тестовых данных
/// </summary>
public class TestDataFixture
{
    /// <summary>
    /// Список пациентов
    /// </summary>
    public List<Patient> Patients { get; } = TestData.Patients;

    /// <summary>
    /// Список врачей
    /// </summary>
    public List<Doctor> Doctors { get; } = TestData.Doctors;

    /// <summary>
    /// Список записей на прием
    /// </summary>
    public List<Appointment> Appointments { get; } = TestData.Appointments;

    /// <summary>
    /// Список специализаций
    /// </summary>
    public List<Specialization> Specializations { get; } = TestData.Specializations;
}