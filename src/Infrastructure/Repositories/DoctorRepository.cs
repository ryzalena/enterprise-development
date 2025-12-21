using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly ApplicationDbContext _context;

    public DoctorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Doctor>> GetAllAsync()
    {
        return await _context.Doctors
            .Include(d => d.Specialization)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Doctor?> GetByIdAsync(int id)
    {
        return await _context.Doctors
            .Include(d => d.Specialization)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Doctor> AddAsync(Doctor doctor)
    {
        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();
        return doctor;
    }

    public async Task UpdateAsync(Doctor doctor)
    {
        _context.Entry(doctor).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var doctor = await GetByIdAsync(id);
        if (doctor != null)
        {
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Doctor>> GetBySpecializationAsync(int specializationId)
    {
        return await _context.Doctors
            .Include(d => d.Specialization)
            .Where(d => d.SpecializationId == specializationId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Doctor>> GetWithExperienceAsync(int minExperienceYears)
    {
        return await _context.Doctors
            .Include(d => d.Specialization)
            .Where(d => d.ExperienceYears >= minExperienceYears)
            .AsNoTracking()
            .ToListAsync();
    }
}