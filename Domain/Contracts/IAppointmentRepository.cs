using policlinicApp.Domain.Entities;

namespace policlinicApp.Domain.Entities;

public interface IAppointmentRepository
{
    Appointment GetById(int id);
    IEnumerable<Appointment> GetAll();
    IEnumerable<Appointment> GetAppointmentsByPatient(string patientPassport);
    IEnumerable<Appointment> GetAppointmentsByDoctor(string doctorPassport);
    IEnumerable<Appointment> GetAppointmentsByRoom(string roomNumber);
    IEnumerable<Appointment> GetAppointmentsByDateRange(DateTime startDate, DateTime endDate);
    IEnumerable<Appointment> GetFollowUpAppointments();
    int GetFollowUpAppointmentsCountLastMonth();
    void Add(Appointment appointment);
    void Update(Appointment appointment);
    void Delete(int id);
}