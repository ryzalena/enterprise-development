using Domain.Entities;
using Domain.Interfaces;
using Domain.TestData;

namespace Application.Services;

public class PatientService : IPatientService
{
    private readonly List<Patient> _patients;
    private readonly List<Appointment> _appointments;

    public PatientService()
    {
        _patients = TestData.Patients;
        _appointments = TestData.Appointments;
    }

    public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
    {
        await Task.CompletedTask;
        return _patients;
    }

    public async Task<Patient?> GetPatientByIdAsync(int id)
    {
        await Task.CompletedTask;
        return _patients.FirstOrDefault(p => p.Id == id);
    }

    public async Task<IEnumerable<Patient>> GetPatientsByDoctorAsync(int doctorId)
    {
        await Task.CompletedTask;
        
        var patientIds = _appointments
            .Where(a => a.DoctorId == doctorId)
            .Select(a => a.PatientId)
            .Distinct();
            
        return _patients.Where(p => patientIds.Contains(p.Id));
    }

    public async Task<IEnumerable<Patient>> GetPatientsOverAgeAsync(int age)
    {
        await Task.CompletedTask;
        
        var cutoffDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-age));
        return _patients.Where(p => p.BirthDate <= cutoffDate);
    }

    public async Task<Patient> CreatePatientAsync(Patient patient)
    {
        await Task.CompletedTask;
        
        var newId = _patients.Max(p => p.Id) + 1;
        patient.Id = newId;
        _patients.Add(patient);
        
        return patient;
    }

    public async Task UpdatePatientAsync(int id, Patient patient)
    {
        await Task.CompletedTask;
        
        var existingPatient = _patients.FirstOrDefault(p => p.Id == id);
        if (existingPatient == null)
            throw new ArgumentException("Patient not found");

        existingPatient.PassportNumber = patient.PassportNumber;
        existingPatient.FullName = patient.FullName;
        existingPatient.BirthDate = patient.BirthDate;
        existingPatient.Address = patient.Address;
        existingPatient.PhoneNumber = patient.PhoneNumber;
    }

    public async Task DeletePatientAsync(int id)
    {
        await Task.CompletedTask;
        
        var patient = _patients.FirstOrDefault(p => p.Id == id);
        if (patient != null)
        {
            _patients.Remove(patient);
        }
    }
}