using Domain.Entities;

namespace Domain.Interfaces;

public interface ISpecializationService
{
    Task<IEnumerable<Specialization>> GetAllSpecializationsAsync();
    Task<Specialization?> GetSpecializationByIdAsync(int id);
    Task<Specialization> CreateSpecializationAsync(Specialization specialization);
    Task UpdateSpecializationAsync(int id, Specialization specialization);
    Task DeleteSpecializationAsync(int id);
}
