using Domain.Entities;
using Domain.TestData;

namespace Application.Services;

public class PolyclinicService : IPolyclinicService
{
    private readonly List<Patient> _patients;
    private readonly List<Doctor> _doctors;
    private readonly List<Appointment> _appointments;
    private readonly List<Specialization> _specializations;

    public PolyclinicService()
    {
        _patients = TestData.Patients;
        _doctors = TestData.Doctors;
        _appointments = TestData.Appointments;
        _specializations = TestData.Specializations;
    }

    // CRUD операции для пациентов
    public Task<List<Patient>> GetPatientsAsync() => Task.FromResult(_patients);
    
    public Task<Patient?> GetPatientByIdAsync(int id) => 
        Task.FromResult(_patients.FirstOrDefault(p => p.Id == id));
    
    public Task<Patient> CreatePatientAsync(Patient patient)
    {
        var newId = _patients.Count > 0 ? _patients.Max(p => p.Id) + 1 : 1;
        patient.Id = newId;
        _patients.Add(patient);
        return Task.FromResult(patient);
    }
    
    public Task<Patient?> UpdatePatientAsync(int id, Patient patient)
    {
        var existing = _patients.FirstOrDefault(p => p.Id == id);
        if (existing != null)
        {
            existing.PassportNumber = patient.PassportNumber;
            existing.FullName = patient.FullName;
            existing.Gender = patient.Gender;
            existing.BirthDate = patient.BirthDate;
            existing.Address = patient.Address;
            existing.BloodGroup = patient.BloodGroup;
            existing.RhFactor = patient.RhFactor;
            existing.PhoneNumber = patient.PhoneNumber;
        }
        return Task.FromResult(existing);
    }
    
    public Task<bool> DeletePatientAsync(int id)
    {
        var patient = _patients.FirstOrDefault(p => p.Id == id);
        if (patient != null)
        {
            _patients.Remove(patient);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    // CRUD операции для врачей
    public Task<List<Doctor>> GetDoctorsAsync() => Task.FromResult(_doctors);
    
    public Task<Doctor?> GetDoctorByIdAsync(int id) => 
        Task.FromResult(_doctors.FirstOrDefault(d => d.Id == id));
    
    public Task<Doctor> CreateDoctorAsync(Doctor doctor)
    {
        var newId = _doctors.Count > 0 ? _doctors.Max(d => d.Id) + 1 : 1;
        doctor.Id = newId;
        
        // Находим специализацию по имени или создаем новую
        var specialization = _specializations.FirstOrDefault(s => s.Name == doctor.Specialization.Name);
        if (specialization != null)
        {
            doctor.Specialization = specialization;
        }
        else
        {
            var specId = _specializations.Count > 0 ? _specializations.Max(s => s.Id) + 1 : 1;
            doctor.Specialization.Id = specId;
            _specializations.Add(doctor.Specialization);
        }
        
        _doctors.Add(doctor);
        return Task.FromResult(doctor);
    }
    
    public Task<Doctor?> UpdateDoctorAsync(int id, Doctor doctor)
    {
        var existing = _doctors.FirstOrDefault(d => d.Id == id);
        if (existing != null)
        {
            existing.PassportNumber = doctor.PassportNumber;
            existing.FullName = doctor.FullName;
            existing.BirthYear = doctor.BirthYear;
            existing.ExperienceYears = doctor.ExperienceYears;
            
            // Обновление специализации
            var specialization = _specializations.FirstOrDefault(s => s.Name == doctor.Specialization.Name);
            if (specialization != null)
            {
                existing.Specialization = specialization;
            }
            else
            {
                var specId = _specializations.Count > 0 ? _specializations.Max(s => s.Id) + 1 : 1;
                doctor.Specialization.Id = specId;
                _specializations.Add(doctor.Specialization);
                existing.Specialization = doctor.Specialization;
            }
        }
        return Task.FromResult(existing);
    }
    
    public Task<bool> DeleteDoctorAsync(int id)
    {
        var doctor = _doctors.FirstOrDefault(d => d.Id == id);
        if (doctor != null)
        {
            _doctors.Remove(doctor);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    // CRUD операции для записей на прием
    public Task<List<Appointment>> GetAppointmentsAsync() => Task.FromResult(_appointments);
    
    public Task<Appointment?> GetAppointmentByIdAsync(int id) => 
        Task.FromResult(_appointments.FirstOrDefault(a => a.Id == id));
    
    public Task<Appointment> CreateAppointmentAsync(Appointment appointment)
    {
        var newId = _appointments.Count > 0 ? _appointments.Max(a => a.Id) + 1 : 1;
        appointment.Id = newId;
        
        // Проверяем существование пациента и врача
        var patient = _patients.FirstOrDefault(p => p.Id == appointment.PatientId);
        var doctor = _doctors.FirstOrDefault(d => d.Id == appointment.DoctorId);
        
        if (patient == null || doctor == null)
        {
            throw new ArgumentException("Patient or Doctor not found");
        }
        
        appointment.Patient = patient;
        appointment.Doctor = doctor;
        
        _appointments.Add(appointment);
        return Task.FromResult(appointment);
    }
    
    public Task<Appointment?> UpdateAppointmentAsync(int id, Appointment appointment)
    {
        var existing = _appointments.FirstOrDefault(a => a.Id == id);
        if (existing != null)
        {
            // Проверяем существование пациента и врача
            var patient = _patients.FirstOrDefault(p => p.Id == appointment.PatientId);
            var doctor = _doctors.FirstOrDefault(d => d.Id == appointment.DoctorId);
            
            if (patient == null || doctor == null)
            {
                throw new ArgumentException("Patient or Doctor not found");
            }
            
            existing.PatientId = appointment.PatientId;
            existing.DoctorId = appointment.DoctorId;
            existing.AppointmentDateTime = appointment.AppointmentDateTime;
            existing.RoomNumber = appointment.RoomNumber;
            existing.IsFollowUp = appointment.IsFollowUp;
            existing.Patient = patient;
            existing.Doctor = doctor;
        }
        return Task.FromResult(existing);
    }
    
    public Task<bool> DeleteAppointmentAsync(int id)
    {
        var appointment = _appointments.FirstOrDefault(a => a.Id == id);
        if (appointment != null)
        {
            _appointments.Remove(appointment);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    // CRUD операции для специализаций
    public Task<List<Specialization>> GetSpecializationsAsync() => Task.FromResult(_specializations);
    
    public Task<Specialization?> GetSpecializationByIdAsync(int id) => 
        Task.FromResult(_specializations.FirstOrDefault(s => s.Id == id));

    // Дополнительные методы
    public Task<List<Appointment>> GetAppointmentsByPatientAsync(int patientId) =>
        Task.FromResult(_appointments
            .Where(a => a.PatientId == patientId)
            .OrderBy(a => a.AppointmentDateTime)
            .ToList());
    
    public Task<List<Appointment>> GetAppointmentsByDoctorAsync(int doctorId) =>
        Task.FromResult(_appointments
            .Where(a => a.DoctorId == doctorId)
            .OrderBy(a => a.AppointmentDateTime)
            .ToList());

    // Аналитические запросы
    public Task<List<Doctor>> GetDoctorsWithExperienceAsync(int minExperienceYears) =>
        Task.FromResult(_doctors
            .Where(d => d.ExperienceYears >= minExperienceYears)
            .OrderBy(d => d.Id)
            .ToList());

    public Task<List<Patient>> GetPatientsByDoctorAsync(int doctorId) =>
        Task.FromResult(_appointments
            .Where(a => a.DoctorId == doctorId)
            .Select(a => a.Patient)
            .Distinct()
            .OrderBy(p => p.FullName)
            .ToList());

    public Task<int> GetFollowUpAppointmentsCountLastMonthAsync()
    {
        var lastMonth = DateTime.Now.AddMonths(-1);
        return Task.FromResult(_appointments
            .Count(a => a.IsFollowUp && 
                       a.AppointmentDateTime.Month == lastMonth.Month && 
                       a.AppointmentDateTime.Year == lastMonth.Year));
    }

    public Task<List<Patient>> GetPatientsOver30WithMultipleDoctorsAsync()
    {
        var referenceDate = DateTime.Now;
        return Task.FromResult(_appointments
            .Where(a => a.Patient.BirthDate <= DateOnly.FromDateTime(referenceDate.AddYears(-30)))
            .GroupBy(a => a.Patient)
            .Where(g => g.Select(a => a.DoctorId).Distinct().Count() > 1)
            .Select(g => g.Key)
            .OrderBy(p => p.BirthDate)
            .ToList());
    }

    public Task<List<Appointment>> GetAppointmentsInRoomForCurrentMonthAsync(string roomNumber)
    {
        var now = DateTime.Now;
        return Task.FromResult(_appointments
            .Where(a => a.RoomNumber == roomNumber && 
                       a.AppointmentDateTime.Month == now.Month && 
                       a.AppointmentDateTime.Year == now.Year)
            .OrderBy(a => a.Id)
            .ToList());
    }
}