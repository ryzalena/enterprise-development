using Domain.Entities;

namespace Domain.Interfaces;

public interface IAppointmentService
{
    public Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
    public Task<Appointment?> GetAppointmentByIdAsync(int id);
    public Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId);
    public Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId);
    public Task<IEnumerable<Appointment>> GetAppointmentsByRoomAndDateAsync(string roomNumber, DateTime date);
    public Task<int> GetFollowUpCountLastMonthAsync();
    public Task<Appointment> CreateAppointmentAsync(Appointment appointment);
    public Task UpdateAppointmentAsync(int id, Appointment appointment);
    public Task DeleteAppointmentAsync(int id);
}
