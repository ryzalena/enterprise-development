using Domain.Entities;

namespace Domain.Interfaces;

public interface IDoctorService
{
    public Task<IEnumerable<Doctor>> GetAllDoctorsAsync();
    public Task<Doctor?> GetDoctorByIdAsync(int id);
    public Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(int specializationId);
    public Task<IEnumerable<Doctor>> GetDoctorsWithExperienceAsync(int minExperienceYears);
    public Task<Doctor> CreateDoctorAsync(Doctor doctor);
    public Task UpdateDoctorAsync(int id, Doctor doctor);
    public Task DeleteDoctorAsync(int id);
}
