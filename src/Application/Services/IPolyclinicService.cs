using Domain.Entities;

namespace Application.Services;

public interface IPolyclinicService
{
    // CRUD операции для пациентов
    Task<List<Patient>> GetPatientsAsync();
    Task<Patient?> GetPatientByIdAsync(int id);
    Task<Patient> CreatePatientAsync(Patient patient);
    Task<Patient?> UpdatePatientAsync(int id, Patient patient);
    Task<bool> DeletePatientAsync(int id);
    
    // CRUD операции для врачей
    Task<List<Doctor>> GetDoctorsAsync();
    Task<Doctor?> GetDoctorByIdAsync(int id);
    Task<Doctor> CreateDoctorAsync(Doctor doctor);
    Task<Doctor?> UpdateDoctorAsync(int id, Doctor doctor);
    Task<bool> DeleteDoctorAsync(int id);
    
    // CRUD операции для записей
    Task<List<Appointment>> GetAppointmentsAsync();
    Task<Appointment?> GetAppointmentByIdAsync(int id);
    Task<Appointment> CreateAppointmentAsync(Appointment appointment);
    Task<Appointment?> UpdateAppointmentAsync(int id, Appointment appointment);
    Task<bool> DeleteAppointmentAsync(int id);
    
    // CRUD операции для специализаций
    Task<List<Specialization>> GetSpecializationsAsync();
    Task<Specialization?> GetSpecializationByIdAsync(int id);
    
    // Дополнительные методы
    Task<List<Appointment>> GetAppointmentsByPatientAsync(int patientId);
    Task<List<Appointment>> GetAppointmentsByDoctorAsync(int doctorId);
    
    // Аналитические запросы (из тестов)
    Task<List<Doctor>> GetDoctorsWithExperienceAsync(int minExperienceYears);
    Task<List<Patient>> GetPatientsByDoctorAsync(int doctorId);
    Task<int> GetFollowUpAppointmentsCountLastMonthAsync();
    Task<List<Patient>> GetPatientsOver30WithMultipleDoctorsAsync();
    Task<List<Appointment>> GetAppointmentsInRoomForCurrentMonthAsync(string roomNumber);
}