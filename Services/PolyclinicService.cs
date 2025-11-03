using policlinicApp.Domain.Entities;
using policlinicApp.Services.Interfaces;

namespace policlinicApp.Services;

public class PolyclinicService : IPolyclinicService
{
    private readonly List<Patient> _patients;
    private readonly List<Doctor> _doctors;
    private readonly List<Appointment> _appointments;
    private readonly List<Specialization> _specializations;

    public PolyclinicService(List<Patient> patients, List<Doctor> doctors, 
                           List<Appointment> appointments, List<Specialization> specializations)
    {
        _patients = patients;
        _doctors = doctors;
        _appointments = appointments;
        _specializations = specializations;
    }

    public IEnumerable<Doctor> GetDoctorsWithExperience(int minExperienceYears)
    {
        return _doctors
            .Where(d => d.ExperienceYears >= minExperienceYears)
            .ToList();
    }

    public IEnumerable<Patient> GetPatientsByDoctorOrderedByName(string doctorPassport)
    {
        return _appointments
            .Where(a => a.Doctor.PassportNumber == doctorPassport)
            .Select(a => a.Patient)
            .Distinct()
            .OrderBy(p => p.FullName)
            .ToList();
    }

    public int GetFollowUpAppointmentsCountLastMonth()
    {
        var lastMonth = DateTime.Now.AddMonths(-1);
        return _appointments
            .Count(a => a.IsFollowUp && 
                       a.AppointmentDateTime.Month == lastMonth.Month && 
                       a.AppointmentDateTime.Year == lastMonth.Year);
    }

    public IEnumerable<Patient> GetPatientsOver30WithMultipleDoctors()
    {
        return _appointments
            .Where(a => a.Patient.Age > 30)
            .GroupBy(a => a.Patient)
            .Where(g => g.Select(a => a.Doctor.PassportNumber).Distinct().Count() > 1)
            .Select(g => g.Key)
            .OrderBy(p => p.BirthDate)
            .ToList();
    }

    public IEnumerable<Appointment> GetAppointmentsInRoomForCurrentMonth(string roomNumber)
    {
        var currentDate = DateTime.Now;
        return _appointments
            .Where(a => a.RoomNumber == roomNumber && 
                       a.AppointmentDateTime.Month == currentDate.Month && 
                       a.AppointmentDateTime.Year == currentDate.Year)
            .ToList();
    }
}