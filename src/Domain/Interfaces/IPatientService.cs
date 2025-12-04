using Domain.Entities;

namespace Domain.Interfaces;

public interface IPatientService
{
    public Task<IEnumerable<Patient>> GetAllPatientsAsync();
    public Task<Patient?> GetPatientByIdAsync(int id);
    public Task<IEnumerable<Patient>> GetPatientsByDoctorAsync(int doctorId);
    public Task<IEnumerable<Patient>> GetPatientsOverAgeAsync(int age);
    public Task<Patient> CreatePatientAsync(Patient patient);
    public Task UpdatePatientAsync(int id, Patient patient);
    public Task DeletePatientAsync(int id);
}
