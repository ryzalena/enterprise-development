using Domain.Entities;
using Domain.Interfaces;
using Domain.TestData;

namespace Application.Services;

public class DoctorService : IDoctorService
{ 
    private readonly List<Doctor> _doctors;
    private int _nextId;

    public DoctorService()
    {
        _doctors = TestData.Doctors;
        _nextId = _doctors.Count > 0 ? _doctors.Max(d => d.Id) + 1 : 1;
    }

    public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync()
    {
        return await Task.FromResult(_doctors);
    }

    public async Task<Doctor?> GetDoctorByIdAsync(int id)
    {
        return await Task.FromResult(_doctors.FirstOrDefault(d => d.Id == id));
    }

    public async Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(int specializationId)
    {
        return await Task.FromResult(_doctors.Where(d => d.SpecializationId == specializationId));
    }

    public async Task<IEnumerable<Doctor>> GetDoctorsWithExperienceAsync(int minExperienceYears)
    {
        return await Task.FromResult(_doctors.Where(d => d.ExperienceYears >= minExperienceYears));
    }

    public async Task<Doctor> CreateDoctorAsync(Doctor doctor)
    {
        doctor.Id = _nextId++;
        _doctors.Add(doctor);
        
        return await Task.FromResult(doctor);
    }

    public async Task UpdateDoctorAsync(int id, Doctor doctor)
    {
        var existingDoctor = _doctors.FirstOrDefault(d => d.Id == id) 
                             ?? throw new ArgumentException("Doctor not found");

        existingDoctor.PassportNumber = doctor.PassportNumber;
        existingDoctor.FullName = doctor.FullName;
        existingDoctor.BirthYear = doctor.BirthYear;
        existingDoctor.ExperienceYears = doctor.ExperienceYears;
        existingDoctor.SpecializationId = doctor.SpecializationId;

        await Task.CompletedTask;
    }

    public async Task DeleteDoctorAsync(int id)
    {
        var doctor = _doctors.FirstOrDefault(d => d.Id == id);
        if (doctor != null)
        {
            _doctors.Remove(doctor);
        }

        await Task.CompletedTask;
    }
}