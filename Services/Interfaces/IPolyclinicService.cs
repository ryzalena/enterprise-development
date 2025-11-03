using policlinicApp.Domain.Entities;

namespace policlinicApp.Services.Interfaces;

public interface IPolyclinicService
{
    IEnumerable<Doctor> GetDoctorsWithExperience(int minExperienceYears);
    IEnumerable<Patient> GetPatientsByDoctorOrderedByName(string doctorPassport);
    int GetFollowUpAppointmentsCountLastMonth();
    IEnumerable<Patient> GetPatientsOver30WithMultipleDoctors();
    IEnumerable<Appointment> GetAppointmentsInRoomForCurrentMonth(string roomNumber);
}