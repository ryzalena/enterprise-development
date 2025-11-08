using Domain.Enums;

namespace Domain.Entities;

public class Patient
{
    public required string PassportNumber { get; set; }
    public required string FullName { get; set; }
    public required Gender Gender { get; set; }
    public required DateTime BirthDate { get; set; }
    public required string Address { get; set; }
    public required BloodGroup BloodGroup { get; set; }
    public required RhFactor RhFactor { get; set; }
    public required string PhoneNumber { get; set; }

    public int Age => DateTime.Now.Year - BirthDate.Year;
}