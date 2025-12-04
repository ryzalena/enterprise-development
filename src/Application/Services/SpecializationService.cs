using Domain.Entities;
using Domain.Interfaces;
using Domain.TestData;

namespace Application.Services;

public class SpecializationService : ISpecializationService
{
    private readonly List<Specialization> _specializations;
    private int _nextId;

    public SpecializationService()
    {
        _specializations = TestData.Specializations;
        _nextId = _specializations.Count > 0 ? _specializations.Max(s => s.Id) + 1 : 1;
    }

    public async Task<IEnumerable<Specialization>> GetAllSpecializationsAsync()
    {
        return await Task.FromResult(_specializations);
    }

    public async Task<Specialization?> GetSpecializationByIdAsync(int id)
    {
        return await Task.FromResult(_specializations.FirstOrDefault(s => s.Id == id));
    }

    public async Task<Specialization> CreateSpecializationAsync(Specialization specialization)
    {
        specialization.Id = _nextId++;
        _specializations.Add(specialization);
        
        return await Task.FromResult(specialization);
    }

    public async Task UpdateSpecializationAsync(int id, Specialization specialization)
    {
        var existingSpecialization = _specializations.FirstOrDefault(s => s.Id == id) 
                                     ?? throw new ArgumentException("Specialization not found");

        existingSpecialization.Name = specialization.Name;
        existingSpecialization.Description = specialization.Description;

        await Task.CompletedTask;
    }

    public async Task DeleteSpecializationAsync(int id)
    {
        var specialization = _specializations.FirstOrDefault(s => s.Id == id);
        if (specialization != null)
        {
            _specializations.Remove(specialization);
        }

        await Task.CompletedTask;
    }
}