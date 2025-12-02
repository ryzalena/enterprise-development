namespace Application.Dtos;

public class PatientManipulationDto
{
    public required string PassportNumber { get; set; }
    public required string FullName { get; set; }
    public required string Gender { get; set; }
    public required DateOnly BirthDate { get; set; }
    public required string Address { get; set; }
    public required string BloodGroup { get; set; }
    public required string RhFactor { get; set; }
    public required string PhoneNumber { get; set; }
}