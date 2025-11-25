namespace Application.Dtos;

public class AppointmentDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public bool IsFollowUp { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
}

public class CreateAppointmentDto
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public bool IsFollowUp { get; set; }
}

public class UpdateAppointmentDto
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public bool IsFollowUp { get; set; }
}