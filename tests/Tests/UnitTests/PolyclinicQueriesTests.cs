using Domain.Entities;
using Domain.Enums;
using Domain.TestData;
using Xunit;

namespace Tests.UnitTests;

/// <summary>
/// Тесты для проверки LINQ запросов поликлиники
/// </summary>
public class PolyclinicQueriesTests
{
    private readonly List<Patient> _patients;
    private readonly List<Doctor> _doctors;
    private readonly List<Appointment> _appointments;
    private readonly List<Specialization> _specializations;

    /// <summary>
    /// Инициализация тестовых данных
    /// </summary>
    public PolyclinicQueriesTests()
    {
        _patients = TestData.Patients;
        _doctors = TestData.Doctors;
        _appointments = TestData.Appointments;
        _specializations = TestData.Specializations;
    }

    /// <summary>
    /// Тест: Получение врачей со стажем работы не менее указанного количества лет
    /// </summary>
    [Fact]
    public void GetDoctorsWithExperience_WhenMinExperience10Years_ReturnsDoctorsWithAtLeast10YearsExperience()
    {
        // Arrange
        var minExperience = 10;

        // Act
        var result = _doctors
            .Where(d => d.ExperienceYears >= minExperience)
            .ToList();

        // Assert
        Assert.NotNull(result);
        Assert.All(result, d => Assert.True(d.ExperienceYears >= minExperience));
    }

    /// <summary>
    /// Тест: Получение пациентов конкретного врача, отсортированных по ФИО
    /// </summary>
    [Fact]
    public void GetPatientsByDoctorOrderedByName_WhenValidDoctorPassport_ReturnsOrderedPatients()
    {
        // Arrange
        var doctorPassport = "CD200001";

        // Act
        var result = _appointments
            .Where(a => a.Doctor.PassportNumber == doctorPassport)
            .Select(a => a.Patient)
            .Distinct()
            .OrderBy(p => p.FullName)
            .ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    /// <summary>
    /// Тест: Подсчет количества повторных приемов за последний месяц
    /// </summary>
    [Fact]
    public void GetFollowUpAppointmentsCountLastMonth_WhenCalled_ReturnsCorrectCount()
    {
        // Act
        var lastMonth = DateTime.Now.AddMonths(-1);
        var result = _appointments
            .Count(a => a.IsFollowUp && 
                       a.AppointmentDateTime.Month == lastMonth.Month && 
                       a.AppointmentDateTime.Year == lastMonth.Year);

        // Assert
        Assert.Equal(2, result);
    }

    /// <summary>
    /// Тест: Получение пациентов старше 30 лет, записанных к нескольким врачам
    /// </summary>
    [Fact]
    public void GetPatientsOver30WithMultipleDoctors_WhenCalled_ReturnsPatientsOver30WithMultipleDoctorsOrderedByBirthDate()
    {
        // Act
        var result = _appointments
            .Where(a => a.Patient.Age > 30)
            .GroupBy(a => a.Patient)
            .Where(g => g.Select(a => a.Doctor.PassportNumber).Distinct().Count() > 1)
            .Select(g => g.Key)
            .OrderBy(p => p.BirthDate)
            .ToList();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.All(result, p => Assert.True(p.Age > 30));
    }

    /// <summary>
    /// Тест: Получение приемов за текущий месяц в указанном кабинете
    /// </summary>
    [Fact]
    public void GetAppointmentsInRoomForCurrentMonth_WhenValidRoomNumber_ReturnsAppointmentsForCurrentMonth()
    {
        // Arrange
        var roomNumber = "101";
        var currentDate = DateTime.Now;

        // Act
        var result = _appointments
            .Where(a => a.RoomNumber == roomNumber && 
                       a.AppointmentDateTime.Month == currentDate.Month && 
                       a.AppointmentDateTime.Year == currentDate.Year)
            .ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, a => Assert.Equal(roomNumber, a.RoomNumber));
    }
}