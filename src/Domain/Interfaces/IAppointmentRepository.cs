using Domain.Entities;

namespace Domain.Interfaces;

public interface IAppointmentRepository
{
    Task<IEnumerable<Appointment>> GetAllAsync();
    Task<Appointment?> GetByIdAsync(int id);
    Task<Appointment> AddAsync(Appointment appointment);
    Task UpdateAsync(Appointment appointment);
    Task DeleteAsync(int id);
    Task<IEnumerable<Appointment>> GetByDoctorAsync(int doctorId);
    Task<IEnumerable<Appointment>> GetByPatientAsync(int patientId);
    Task<IEnumerable<Appointment>> GetByRoomAndDateAsync(string roomNumber, DateTime date);
    Task<int> GetFollowUpCountLastMonthAsync();
}