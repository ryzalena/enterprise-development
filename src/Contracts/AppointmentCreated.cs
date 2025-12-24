public record AppointmentCreated
{
    public Guid Id { get; init; }
    public int PatientId { get; init; }
    public int DoctorId { get; init; }
    public DateTime AppointmentDate { get; init; }
    public DateTime CreatedAt { get; init; }
}