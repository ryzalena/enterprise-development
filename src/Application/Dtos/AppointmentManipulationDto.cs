namespace Application.Dtos;

public class AppointmentManipulationDto
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public bool IsFollowUp { get; set; }
}