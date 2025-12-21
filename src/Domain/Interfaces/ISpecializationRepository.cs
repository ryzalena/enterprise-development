using Domain.Entities;

namespace Domain.Interfaces;

public interface ISpecializationRepository
{
    Task<IEnumerable<Specialization>> GetAllAsync();
    Task<Specialization?> GetByIdAsync(int id);
    Task<Specialization> AddAsync(Specialization specialization);
    Task UpdateAsync(Specialization specialization);
    Task DeleteAsync(int id);
}
