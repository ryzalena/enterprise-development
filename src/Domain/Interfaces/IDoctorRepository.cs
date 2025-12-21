using Domain.Entities;

namespace Domain.Interfaces;
    
public interface IDoctorRepository
{
    Task<IEnumerable<Doctor>> GetAllAsync();
    Task<Doctor?> GetByIdAsync(int id);
    Task<Doctor> AddAsync(Doctor doctor);
    Task UpdateAsync(Doctor doctor);
    Task DeleteAsync(int id);
    Task<IEnumerable<Doctor>> GetBySpecializationAsync(int specializationId);
    Task<IEnumerable<Doctor>> GetWithExperienceAsync(int minExperienceYears);
}