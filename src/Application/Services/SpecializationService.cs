using Domain.Entities;
using Domain.Interfaces;
using Domain.TestData;

namespace Application.Services;

public class SpecializationService : ISpecializationService
{
    private readonly List<Specialization> _specializations;

    public SpecializationService()
    {
        _specializations = TestData.Specializations;
    }

    public async Task<IEnumerable<Specialization>> GetAllSpecializationsAsync()
    {
        await Task.CompletedTask;
        return _specializations;
    }

    public async Task<Specialization?> GetSpecializationByIdAsync(int id)
    {
        await Task.CompletedTask;
        return _specializations.FirstOrDefault(s => s.Id == id);
    }

    public async Task<Specialization> CreateSpecializationAsync(Specialization specialization)
    {
        await Task.CompletedTask;
        
        var newId = _specializations.Max(s => s.Id) + 1;
        specialization.Id = newId;
        _specializations.Add(specialization);
        
        return specialization;
    }

    public async Task UpdateSpecializationAsync(int id, Specialization specialization)
    {
        await Task.CompletedTask;
        
        var existingSpecialization = _specializations.FirstOrDefault(s => s.Id == id);
        if (existingSpecialization == null)
            throw new ArgumentException("Specialization not found");

        existingSpecialization.Name = specialization.Name;
        existingSpecialization.Description = specialization.Description;
    }

    public async Task DeleteSpecializationAsync(int id)
    {
        await Task.CompletedTask;
        
        var specialization = _specializations.FirstOrDefault(s => s.Id == id);
        if (specialization != null)
        {
            _specializations.Remove(specialization);
        }
    }
}