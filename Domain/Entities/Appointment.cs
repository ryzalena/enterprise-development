using policlinicApp.Domain.Entities;

namespace policlinicApp.Domain.Entities;

public class Appointment
{
    public int Id { get; set; }
    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public string RoomNumber { get; set; }
    public bool IsFollowUp { get; set; }

    public Appointment(int id, Patient patient, Doctor doctor, 
        DateTime appointmentDateTime, string roomNumber, bool isFollowUp)
    {
        Id = id;
        Patient = patient;
        Doctor = doctor;
        AppointmentDateTime = appointmentDateTime;
        RoomNumber = roomNumber;
        IsFollowUp = isFollowUp;
    }    
}