namespace Application.Dtos;

public class PatientDto
{
    public int Id { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string BloodGroup { get; set; } = string.Empty;
    public string RhFactor { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int Age { get; set; }
}

public class CreatePatientDto
{
    public string PassportNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string BloodGroup { get; set; } = string.Empty;
    public string RhFactor { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class UpdatePatientDto
{
    public string PassportNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string BloodGroup { get; set; } = string.Empty;
    public string RhFactor { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}