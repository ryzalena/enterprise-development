using Domain.Entities;
using Domain.Interfaces;
using Domain.TestData;

namespace Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly List<Appointment> _appointments;

    public AppointmentService()
    {
        _appointments = TestData.Appointments;
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
    {
        await Task.CompletedTask;
        return _appointments;
    }

    public async Task<Appointment?> GetAppointmentByIdAsync(int id)
    {
        await Task.CompletedTask;
        return _appointments.FirstOrDefault(a => a.Id == id);
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId)
    {
        await Task.CompletedTask;
        return _appointments.Where(a => a.DoctorId == doctorId);
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId)
    {
        await Task.CompletedTask;
        return _appointments.Where(a => a.PatientId == patientId);
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByRoomAndDateAsync(string roomNumber, DateTime date)
    {
        await Task.CompletedTask;
        return _appointments.Where(a => a.RoomNumber == roomNumber && 
                                       a.AppointmentDateTime.Date == date.Date);
    }

    public async Task<int> GetFollowUpCountLastMonthAsync()
    {
        await Task.CompletedTask;
        
        var firstDayOfLastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
        var lastDayOfLastMonth = firstDayOfLastMonth.AddMonths(1).AddDays(-1);
        
        return _appointments.Count(a => a.IsFollowUp && 
                                       a.AppointmentDateTime >= firstDayOfLastMonth && 
                                       a.AppointmentDateTime <= lastDayOfLastMonth);
    }

    public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
    {
        await Task.CompletedTask;
        
        var newId = _appointments.Max(a => a.Id) + 1;
        appointment.Id = newId;
        _appointments.Add(appointment);
        
        return appointment;
    }

    public async Task UpdateAppointmentAsync(int id, Appointment appointment)
    {
        await Task.CompletedTask;
        
        var existingAppointment = _appointments.FirstOrDefault(a => a.Id == id);
        if (existingAppointment == null)
            throw new ArgumentException("Appointment not found");

        existingAppointment.AppointmentDateTime = appointment.AppointmentDateTime;
        existingAppointment.RoomNumber = appointment.RoomNumber;
        existingAppointment.IsFollowUp = appointment.IsFollowUp;
        existingAppointment.DoctorId = appointment.DoctorId;
        existingAppointment.PatientId = appointment.PatientId;
    }

    public async Task DeleteAppointmentAsync(int id)
    {
        await Task.CompletedTask;
        
        var appointment = _appointments.FirstOrDefault(a => a.Id == id);
        if (appointment != null)
        {
            _appointments.Remove(appointment);
        }
    }
}