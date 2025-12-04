using Domain.Entities;
using Domain.Interfaces;
using Domain.TestData;

namespace Application.Services;

public class PatientService : IPatientService
{
    private readonly List<Patient> _patients;
    private readonly List<Appointment> _appointments;
    private int _nextId;

    public PatientService()
    {
        _patients = TestData.Patients;
        _appointments = TestData.Appointments;
        _nextId = _patients.Count > 0 ? _patients.Max(p => p.Id) + 1 : 1;
    }

    public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
    {
        return await Task.FromResult(_patients);
    }

    public async Task<Patient?> GetPatientByIdAsync(int id)
    {
        return await Task.FromResult(_patients.FirstOrDefault(p => p.Id == id));
    }

    public async Task<IEnumerable<Patient>> GetPatientsByDoctorAsync(int doctorId)
    {
        var patientIds = _appointments
            .Where(a => a.DoctorId == doctorId)
            .Select(a => a.PatientId)
            .Distinct();
            
        return await Task.FromResult(_patients.Where(p => patientIds.Contains(p.Id)));
    }

    public async Task<IEnumerable<Patient>> GetPatientsOverAgeAsync(int age)
    {
        var cutoffDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-age));
        return await Task.FromResult(_patients.Where(p => p.BirthDate <= cutoffDate));
    }

    public async Task<Patient> CreatePatientAsync(Patient patient)
    {
        patient.Id = _nextId++;
        _patients.Add(patient);
        
        return await Task.FromResult(patient);
    }

    public async Task UpdatePatientAsync(int id, Patient patient)
    {
        var existingPatient = _patients.FirstOrDefault(p => p.Id == id) 
            ?? throw new ArgumentException("Patient not found");

        existingPatient.PassportNumber = patient.PassportNumber;
        existingPatient.FullName = patient.FullName;
        existingPatient.Gender = patient.Gender;
        existingPatient.BirthDate = patient.BirthDate;
        existingPatient.Address = patient.Address;
        existingPatient.BloodGroup = patient.BloodGroup;
        existingPatient.RhFactor = patient.RhFactor;
        existingPatient.PhoneNumber = patient.PhoneNumber;

        await Task.CompletedTask;
    }

    public async Task DeletePatientAsync(int id)
    {
        var patient = _patients.FirstOrDefault(p => p.Id == id);
        if (patient != null)
        {
            _patients.Remove(patient);
        }

        await Task.CompletedTask;
    }
}