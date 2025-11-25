// PolyclinicQueriesTests.cs
using Domain.Entities;
using Domain.Enums;
using Domain.TestData;
using Xunit;

namespace Tests.UnitTests;

/// <summary>
/// Тесты для проверки LINQ запросов поликлиники
/// </summary>
/// <remarks>
/// Использует primary constructor для инъекции тестовых данных
/// </remarks>
public class PolyclinicQueriesTests(TestDataFixture fixture) : IClassFixture<TestDataFixture>
{
    private readonly List<Patient> _patients = fixture.Patients;
    private readonly List<Doctor> _doctors = fixture.Doctors;
    private readonly List<Appointment> _appointments = fixture.Appointments;
    private readonly List<Specialization> _specializations = fixture.Specializations;

    /// <summary>
    /// Тест: Получение врачей со стажем работы не менее указанного количества лет
    /// </summary>
    [Fact]
    public void GetDoctorsWithExperience_WhenMinExperience10Years_ReturnsDoctorsWithAtLeast10YearsExperience()
    {
        // Arrange
        const int minExperience = 10;
        
        // Ожидаемые врачи со стажем >= 10 лет 
        var expectedDoctorIds = new List<int> { 1, 2, 3, 4, 6, 7, 8, 9 };

        // Act
        var result = _doctors
            .Where(d => d.ExperienceYears >= minExperience)
            .OrderBy(d => d.Id)
            .ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDoctorIds.Count, result.Count);
        
        var actualDoctorIds = result.Select(d => d.Id).ToList();
        Assert.Equal(expectedDoctorIds, actualDoctorIds);
    }

    /// <summary>
    /// Тест: Получение пациентов конкретного врача, отсортированных по ФИО
    /// </summary>
    [Fact]
    public void GetPatientsByDoctorOrderedByName_WhenValidDoctorId_ReturnsOrderedPatients()
    {
        // Arrange
        var doctorId = 1;
        
        // Ожидаемые пациенты врача с ID 1 
        var expectedPatientIds = new List<int> { 1, 3 };
        var expectedNamesOrder = new List<string> 
        { 
            "Иванов Иван Иванович", 
            "Сидоров Алексей Петрович" 
        };

        // Act
        var result = _appointments
            .Where(a => a.DoctorId == doctorId)
            .Select(a => a.Patient)
            .Distinct()
            .OrderBy(p => p.FullName)
            .ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPatientIds.Count, result.Count);
        
        var actualPatientIds = result.Select(p => p.Id).ToList();
        Assert.Equal(expectedPatientIds, actualPatientIds);
        
        var actualNames = result.Select(p => p.FullName).ToList();
        Assert.Equal(expectedNamesOrder, actualNames);
    }

    /// <summary>
    /// Тест: Подсчет количества повторных приемов за последний месяц
    /// </summary>
    [Fact]
    public void GetFollowUpAppointmentsCountLastMonth_WhenCalled_ReturnsCorrectCount()
    {
        // Arrange
        var referenceDate = new DateTime(2024, 2, 1);
        var lastMonth = referenceDate.AddMonths(-1); // Январь 2024
        
        // Ожидаемое количество повторных приемов 
        const int expectedCount = 1;

        // Act
        var result = _appointments
            .Count(a => a.IsFollowUp && 
                       a.AppointmentDateTime.Month == lastMonth.Month && 
                       a.AppointmentDateTime.Year == lastMonth.Year);

        // Assert
        Assert.Equal(expectedCount, result);
    }

    /// <summary>
    /// Тест: Получение пациентов старше 30 лет, записанных к нескольким врачам
    /// </summary>
    [Fact]
    public void GetPatientsOver30WithMultipleDoctors_WhenCalled_ReturnsPatientsOver30WithMultipleDoctorsOrderedByBirthDate()
    {
        // Arrange
        var referenceDate = new DateTime(2024, 1, 1);
        
        // Ожидаемые пациенты
        var expectedPatientIds = new List<int> { 2, 1 };
        var expectedBirthDateOrder = new List<DateOnly>
        {
            new DateOnly(1975, 8, 20), 
            new DateOnly(1980, 5, 15) 
        };

        // Act
        var result = _appointments
            .Where(a => a.Patient.BirthDate <= DateOnly.FromDateTime(referenceDate.AddYears(-30)))
            .GroupBy(a => a.Patient)
            .Where(g => g.Select(a => a.DoctorId).Distinct().Count() > 1)
            .Select(g => g.Key)
            .OrderBy(p => p.BirthDate)
            .ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPatientIds.Count, result.Count);
        
        var actualPatientIds = result.Select(p => p.Id).ToList();
        Assert.Equal(expectedPatientIds, actualPatientIds);
        
        var actualBirthDates = result.Select(p => p.BirthDate).ToList();
        Assert.Equal(expectedBirthDateOrder, actualBirthDates);
    }

    /// <summary>
    /// Тест: Получение приемов за текущий месяц в указанном кабинете
    /// </summary>
    [Fact]
    public void GetAppointmentsInRoomForCurrentMonth_WhenValidRoomNumber_ReturnsAppointmentsForCurrentMonth()
    {
        // Arrange
        const string roomNumber = "101";
        var referenceDate = new DateTime(2024, 1, 15);
        
        // Ожидаемые записи 
        var expectedAppointmentIds = new List<int> { 1 };
        const int expectedCount = 1;

        // Act
        var result = _appointments
            .Where(a => a.RoomNumber == roomNumber && 
                       a.AppointmentDateTime.Month == referenceDate.Month && 
                       a.AppointmentDateTime.Year == referenceDate.Year)
            .OrderBy(a => a.Id)
            .ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCount, result.Count);
        
        var actualAppointmentIds = result.Select(a => a.Id).ToList();
        Assert.Equal(expectedAppointmentIds, actualAppointmentIds);
    }
}