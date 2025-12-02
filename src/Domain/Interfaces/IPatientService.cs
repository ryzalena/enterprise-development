using Domain.Entities;

namespace Domain.Interfaces;

public interface IPatientService
{
    Task<IEnumerable<Patient>> GetAllPatientsAsync();
    Task<Patient?> GetPatientByIdAsync(int id);
    Task<IEnumerable<Patient>> GetPatientsByDoctorAsync(int doctorId);
    Task<IEnumerable<Patient>> GetPatientsOverAgeAsync(int age);
    Task<Patient> CreatePatientAsync(Patient patient);
    Task UpdatePatientAsync(int id, Patient patient);
    Task DeletePatientAsync(int id);
}
