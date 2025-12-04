using Domain.Entities;

namespace Domain.Interfaces;

public interface ISpecializationService
{
    public Task<IEnumerable<Specialization>> GetAllSpecializationsAsync();
    public Task<Specialization?> GetSpecializationByIdAsync(int id);
    public Task<Specialization> CreateSpecializationAsync(Specialization specialization);
    public Task UpdateSpecializationAsync(int id, Specialization specialization);
    public Task DeleteSpecializationAsync(int id);
}