using Domain.Entities;
using Domain.Interfaces;
using Domain.TestData;

namespace Application.Services;

public class DoctorService : IDoctorService
{
    private readonly List<Doctor> _doctors;
    private readonly List<Specialization> _specializations;

    public DoctorService()
    {
        _doctors = TestData.Doctors;
        _specializations = TestData.Specializations;
    }

    public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync()
    {
        await Task.CompletedTask;
        return _doctors;
    }

    public async Task<Doctor?> GetDoctorByIdAsync(int id)
    {
        await Task.CompletedTask;
        return _doctors.FirstOrDefault(d => d.Id == id);
    }

    public async Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(int specializationId)
    {
        await Task.CompletedTask;
        return _doctors.Where(d => d.SpecializationId == specializationId);
    }

    public async Task<IEnumerable<Doctor>> GetDoctorsWithExperienceAsync(int minExperienceYears)
    {
        await Task.CompletedTask;
        return _doctors.Where(d => d.ExperienceYears >= minExperienceYears);
    }

    public async Task<Doctor> CreateDoctorAsync(Doctor doctor)
    {
        await Task.CompletedTask;
        
        var newId = _doctors.Max(d => d.Id) + 1;
        doctor.Id = newId;
        _doctors.Add(doctor);
        
        return doctor;
    }

    public async Task UpdateDoctorAsync(int id, Doctor doctor)
    {
        await Task.CompletedTask;
        
        var existingDoctor = _doctors.FirstOrDefault(d => d.Id == id);
        if (existingDoctor == null)
            throw new ArgumentException("Doctor not found");

        existingDoctor.PassportNumber = doctor.PassportNumber;
        existingDoctor.FullName = doctor.FullName;
        existingDoctor.BirthYear = doctor.BirthYear;
        existingDoctor.ExperienceYears = doctor.ExperienceYears;
        existingDoctor.SpecializationId = doctor.SpecializationId;
    }

    public async Task DeleteDoctorAsync(int id)
    {
        await Task.CompletedTask;
        
        var doctor = _doctors.FirstOrDefault(d => d.Id == id);
        if (doctor != null)
        {
            _doctors.Remove(doctor);
        }
    }
}