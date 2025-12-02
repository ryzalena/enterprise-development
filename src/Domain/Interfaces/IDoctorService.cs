using Domain.Entities;

namespace Domain.Interfaces;

public interface IDoctorService
{
    Task<IEnumerable<Doctor>> GetAllDoctorsAsync();
    Task<Doctor?> GetDoctorByIdAsync(int id);
    Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(int specializationId);
    Task<IEnumerable<Doctor>> GetDoctorsWithExperienceAsync(int minExperienceYears);
    Task<Doctor> CreateDoctorAsync(Doctor doctor);
    Task UpdateDoctorAsync(int id, Doctor doctor);
    Task DeleteDoctorAsync(int id);
}
