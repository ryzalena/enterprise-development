using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SpecializationRepository : ISpecializationRepository
{
    private readonly ApplicationDbContext _context;

    public SpecializationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Specialization>> GetAllAsync()
    {
        return await _context.Specializations
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Specialization?> GetByIdAsync(int id)
    {
        return await _context.Specializations.FindAsync(id);
    }

    public async Task<Specialization> AddAsync(Specialization specialization)
    {
        _context.Specializations.Add(specialization);
        await _context.SaveChangesAsync();
        return specialization;
    }

    public async Task UpdateAsync(Specialization specialization)
    {
        _context.Entry(specialization).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var specialization = await GetByIdAsync(id);
        if (specialization != null)
        {
            _context.Specializations.Remove(specialization);
            await _context.SaveChangesAsync();
        }
    }
}