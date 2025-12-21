using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly ApplicationDbContext _context;

    public AppointmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync()
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .ThenInclude(d => d.Specialization)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Appointment?> GetByIdAsync(int id)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .ThenInclude(d => d.Specialization)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Appointment> AddAsync(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        _context.Entry(appointment).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var appointment = await GetByIdAsync(id);
        if (appointment != null)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorAsync(int doctorId)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .ThenInclude(d => d.Specialization)
            .Where(a => a.DoctorId == doctorId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByPatientAsync(int patientId)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .ThenInclude(d => d.Specialization)
            .Where(a => a.PatientId == patientId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByRoomAndDateAsync(string roomNumber, DateTime date)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .ThenInclude(d => d.Specialization)
            .Where(a => a.RoomNumber == roomNumber && 
                       a.AppointmentDateTime.Date == date.Date)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> GetFollowUpCountLastMonthAsync()
    {
        var lastMonth = DateTime.Now.AddMonths(-1);
        return await _context.Appointments
            .CountAsync(a => a.IsFollowUp && 
                           a.AppointmentDateTime.Month == lastMonth.Month &&
                           a.AppointmentDateTime.Year == lastMonth.Year);
    }
}