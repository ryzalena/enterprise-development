using Domain.Entities;
using Domain.TestData;
using Xunit;

namespace Tests.UnitTests;

public class PolyclinicQueriesTests
{
    private readonly List<Patient> _patients;
    private readonly List<Doctor> _doctors;
    private readonly List<Appointment> _appointments;
    private readonly List<Specialization> _specializations;

    public PolyclinicQueriesTests()
    {
        _patients = TestData.Patients;
        _doctors = TestData.Doctors;
        _appointments = TestData.Appointments;
        _specializations = TestData.Specializations;
    }

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
        Assert.Equal(8, result.Count);
        Assert.All(result, d => Assert.True(d.ExperienceYears >= minExperience));
    }

    [Fact]
    public void GetPatientsByDoctorOrderedByName_WhenValidDoctorPassport_ReturnsOrderedPatients()
    {
        // Arrange
        var doctorPassport = "CD200001"; // Врачев Александр Петрович

        // Act
        var result = _appointments
            .Where(a => a.Doctor.PassportNumber == doctorPassport)
            .Select(a => a.Patient)
            .Distinct()
            .OrderBy(p => p.FullName)
            .ToList();

        // Assert 
        Assert.NotNull(result);
        Assert.Equal(3, result.Count); 
        var orderedNames = result.Select(p => p.FullName).OrderBy(name => name).ToList();
        Assert.Equal(orderedNames, result.Select(p => p.FullName).ToList());
    }

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
        Assert.Equal(1, result); 
    }

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
        
        foreach (var patient in result)
        {
            var doctorCount = _appointments
                .Where(a => a.Patient.PassportNumber == patient.PassportNumber)
                .Select(a => a.Doctor.PassportNumber)
                .Distinct()
                .Count();
            Assert.True(doctorCount > 1);
        }
    }

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