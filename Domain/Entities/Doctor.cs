using policlinicApp.Domain.Entities;

namespace policlinicApp.Domain.Entities;

public class Doctor
{
    public string PassportNumber { get; set; }
    public string FullName { get; set; }
    public int BirthYear { get; set; }
    public Specialization Specialization { get; set; }
    public int ExperienceYears { get; set; }

    public Doctor(string passportNumber, string fullName, int birthYear, 
        Specialization specialization, int experienceYears)
    {
        PassportNumber = passportNumber;
        FullName = fullName;
        BirthYear = birthYear;
        Specialization = specialization;
        ExperienceYears = experienceYears;
    }
}