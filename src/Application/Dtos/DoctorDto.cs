namespace Application.Dtos;

public class DoctorDto
{
    public int Id { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int BirthYear { get; set; }
    public string SpecializationName { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
}

public class CreateDoctorDto
{
    public string PassportNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int BirthYear { get; set; }
    public string SpecializationName { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
}

public class UpdateDoctorDto
{
    public string PassportNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int BirthYear { get; set; }
    public string SpecializationName { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
}