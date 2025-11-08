namespace Domain.Entities;

public class Doctor
{
    public required string PassportNumber { get; set; }
    public required string FullName { get; set; }
    public required int BirthYear { get; set; }
    public required Specialization Specialization { get; set; }
    public required int ExperienceYears { get; set; }
}