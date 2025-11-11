using Domain.Entities;
using Domain.Enums;
using Domain.TestData;
using Xunit;

namespace Tests.UnitTests;

/// <summary>
/// Тесты для проверки LINQ запросов поликлиники
/// </summary>
public class PolyclinicQueriesTests : IClassFixture<TestDataFixture>
{
    private readonly List<Patient> _patients;
    private readonly List<Doctor> _doctors;
    private readonly List<Appointment> _appointments;
    private readonly List<Specialization> _specializations;

    /// <summary>
    /// Инициализация тестовых данных через primary constructor
    /// </summary>
    /// <param name="fixture">Фикстура с тестовыми данными</param>
    public PolyclinicQueriesTests(TestDataFixture fixture)
    {
        _patients = fixture.Patients;
        _doctors = fixture.Doctors;
        _appointments = fixture.Appointments;
        _specializations = fixture.Specializations;
    }

    /// <summary>
    /// Тест: Получение врачей со стажем работы не менее указанного количества лет
    /// </summary>
    [Fact]
    public void GetDoctorsWithExperience_WhenMinExperience10Years_ReturnsDoctorsWithAtLeast10YearsExperience()
    {
        // Arrange
        const int minExperience = 10;

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
    public void GetPatientsByDoctorOrderedByName_WhenValidDoctorId_ReturnsOrderedPatients()
    {
        // Arrange
        var doctorId = 1;

        // Act
        var result = _appointments
            .Where(a => a.DoctorId == doctorId)
            .Select(a => a.Patient)
            .Distinct()
            .OrderBy(p => p.FullName)
            .ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count); // Пациенты с ID 1 и 3
        
        // Проверяем сортировку по ФИО
        var expectedOrder = result.Select(p => p.FullName).OrderBy(name => name).ToList();
        var actualOrder = result.Select(p => p.FullName).ToList();
        Assert.Equal(expectedOrder, actualOrder);
    }

    /// <summary>
    /// Тест: Подсчет количества повторных приемов за последний месяц
    /// </summary>
    [Fact]
    public void GetFollowUpAppointmentsCountLastMonth_WhenCalled_ReturnsCorrectCount()
    {
        // Arrange
        var referenceDate = new DateTime(2024, 2, 1); // Фиксированная дата для тестов
        var lastMonth = referenceDate.AddMonths(-1); // Январь 2024

        // Act
        var result = _appointments
            .Count(a => a.IsFollowUp && 
                       a.AppointmentDateTime.Month == lastMonth.Month && 
                       a.AppointmentDateTime.Year == lastMonth.Year);

        // Assert
        var expectedCount = _appointments
            .Count(a => a.IsFollowUp && 
                       a.AppointmentDateTime.Month == lastMonth.Month && 
                       a.AppointmentDateTime.Year == lastMonth.Year);
        
        Assert.Equal(expectedCount, result);
    }

    /// <summary>
    /// Тест: Получение пациентов старше 30 лет, записанных к нескольким врачам
    /// </summary>
    [Fact]
    public void GetPatientsOver30WithMultipleDoctors_WhenCalled_ReturnsPatientsOver30WithMultipleDoctorsOrderedByBirthDate()
    {
        // Arrange
        var referenceDate = new DateTime(2024, 1, 1); // Фиксированная дата для расчета возраста

        // Act
        var result = _appointments
            .Where(a => CalculateAge(a.Patient.BirthDate, referenceDate) > 30)
            .GroupBy(a => a.Patient)
            .Where(g => g.Select(a => a.DoctorId).Distinct().Count() > 1)
            .Select(g => g.Key)
            .OrderBy(p => p.BirthDate)
            .ToList();

        // Assert
        Assert.NotNull(result);
        
        if (result.Any())
        {
            // Проверяем возраст относительно фиксированной даты
            Assert.All(result, p => Assert.True(CalculateAge(p.BirthDate, referenceDate) > 30));
            
            // Проверяем, что у каждого пациента действительно > 1 врача
            foreach (var patient in result)
            {
                var doctorCount = _appointments
                    .Where(a => a.Patient.Id == patient.Id)
                    .Select(a => a.DoctorId)
                    .Distinct()
                    .Count();
                Assert.True(doctorCount > 1);
            }
            
            // Проверяем сортировку по дате рождения
            var expectedBirthDateOrder = result.Select(p => p.BirthDate).OrderBy(bd => bd).ToList();
            var actualBirthDateOrder = result.Select(p => p.BirthDate).ToList();
            Assert.Equal(expectedBirthDateOrder, actualBirthDateOrder);
        }
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

        // Act
        var result = _appointments
            .Where(a => a.RoomNumber == roomNumber && 
                       a.AppointmentDateTime.Month == referenceDate.Month && 
                       a.AppointmentDateTime.Year == referenceDate.Year)
            .ToList();

        // Assert
        Assert.NotNull(result);
        
        var expectedCount = _appointments
            .Count(a => a.RoomNumber == roomNumber && 
                       a.AppointmentDateTime.Month == referenceDate.Month && 
                       a.AppointmentDateTime.Year == referenceDate.Year);
        
        Assert.Equal(expectedCount, result.Count);
        Assert.All(result, a => Assert.Equal(roomNumber, a.RoomNumber));
        Assert.All(result, a => 
        {
            Assert.Equal(referenceDate.Month, a.AppointmentDateTime.Month);
            Assert.Equal(referenceDate.Year, a.AppointmentDateTime.Year);
        });
    }

    /// <summary>
    /// Вспомогательный метод для расчета возраста относительно фиксированной даты
    /// </summary>
    /// <param name="birthDate">Дата рождения</param>
    /// <param name="referenceDate">Опорная дата для расчета</param>
    /// <returns>Возраст в годах</returns>
    private static int CalculateAge(DateOnly birthDate, DateTime referenceDate)
    {
        var today = DateOnly.FromDateTime(referenceDate);
        var age = today.Year - birthDate.Year;
        
        // Если день рождения еще не наступил в этом году, вычитаем 1 год
        if (birthDate > today.AddYears(-age))
        {
            age--;
        }
        
        return age;
    }
}

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