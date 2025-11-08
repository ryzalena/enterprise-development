namespace Domain.Entities;

public class Appointment
{
    public required int Id { get; set; }
    public required Patient Patient { get; set; }
    public required Doctor Doctor { get; set; }
    public required DateTime AppointmentDateTime { get; set; }
    public required string RoomNumber { get; set; }
    public required bool IsFollowUp { get; set; }
}