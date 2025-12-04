using Domain.Entities;
using Domain.Interfaces;
using Domain.TestData;

namespace Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly List<Appointment> _appointments;
    private int _nextId;

    public AppointmentService()
    {
        _appointments = TestData.Appointments;
        _nextId = _appointments.Count > 0 ? _appointments.Max(a => a.Id) + 1 : 1;
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
    {
        return await Task.FromResult(_appointments);
    }

    public async Task<Appointment?> GetAppointmentByIdAsync(int id)
    {
        return await Task.FromResult(_appointments.FirstOrDefault(a => a.Id == id));
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId)
    {
        return await Task.FromResult(_appointments.Where(a => a.DoctorId == doctorId));
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId)
    {
        return await Task.FromResult(_appointments.Where(a => a.PatientId == patientId));
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByRoomAndDateAsync(string roomNumber, DateTime date)
    {
        return await Task.FromResult(_appointments.Where(a => a.RoomNumber == roomNumber && 
                                       a.AppointmentDateTime.Date == date.Date));
    }

    public async Task<int> GetFollowUpCountLastMonthAsync()
    {
        var firstDayOfLastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
        var lastDayOfLastMonth = firstDayOfLastMonth.AddMonths(1).AddDays(-1);
        
        return await Task.FromResult(_appointments.Count(a => a.IsFollowUp && 
                                       a.AppointmentDateTime >= firstDayOfLastMonth && 
                                       a.AppointmentDateTime <= lastDayOfLastMonth));
    }

    public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
    {
        appointment.Id = _nextId++;
        _appointments.Add(appointment);
        
        return await Task.FromResult(appointment);
    }

    public async Task UpdateAppointmentAsync(int id, Appointment appointment)
    {
        var existingAppointment = _appointments.FirstOrDefault(a => a.Id == id) 
            ?? throw new ArgumentException("Appointment not found");

        existingAppointment.AppointmentDateTime = appointment.AppointmentDateTime;
        existingAppointment.RoomNumber = appointment.RoomNumber;
        existingAppointment.IsFollowUp = appointment.IsFollowUp;
        existingAppointment.DoctorId = appointment.DoctorId;
        existingAppointment.PatientId = appointment.PatientId;

        await Task.CompletedTask;
    }

    public async Task DeleteAppointmentAsync(int id)
    {
        var appointment = _appointments.FirstOrDefault(a => a.Id == id);
        if (appointment != null)
        {
            _appointments.Remove(appointment);
        }

        await Task.CompletedTask;
    }
}