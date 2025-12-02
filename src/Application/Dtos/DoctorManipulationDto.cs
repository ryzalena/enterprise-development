namespace Application.Dtos;

public class DoctorManipulationDto
{
    public required string PassportNumber { get; set; }
    public required string FullName { get; set; }
    public int BirthYear { get; set; }
    public int SpecializationId { get; set; }
    public int ExperienceYears { get; set; }
}