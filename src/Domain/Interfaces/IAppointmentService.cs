using Domain.Entities;

namespace Domain.Interfaces;

public interface IAppointmentService
{
    Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
    Task<Appointment?> GetAppointmentByIdAsync(int id);
    Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId);
    Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId);
    Task<IEnumerable<Appointment>> GetAppointmentsByRoomAndDateAsync(string roomNumber, DateTime date);
    Task<int> GetFollowUpCountLastMonthAsync();
    Task<Appointment> CreateAppointmentAsync(Appointment appointment);
    Task UpdateAppointmentAsync(int id, Appointment appointment);
    Task DeleteAppointmentAsync(int id);
}
