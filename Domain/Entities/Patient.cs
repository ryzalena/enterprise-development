using policlinicApp.Domain.Enums;

namespace policlinicApp.Domain.Entities;

public class Patient
{
    public string PassportNumber { get; set; }
    public string FullName { get; set; }
    public Gender Gender { get; set; }
    public DateTime BirthDate { get; set; }
    public string Address { get; set; }
    public BloodGroup BloodGroup { get; set; }
    public RhFactor RhFactor { get; set; }
    public string PhoneNumber { get; set; }

    public int Age => DateTime.Now.Year - BirthDate.Year;

    public Patient(string passportNumber, string fullName, Gender gender, 
        DateTime birthDate, string address, BloodGroup bloodGroup, 
        RhFactor rhFactor, string phoneNumber)
    {
        PassportNumber = passportNumber;
        FullName = fullName;
        Gender = gender;
        BirthDate = birthDate;
        Address = address;
        BloodGroup = bloodGroup;
        RhFactor = rhFactor;
        PhoneNumber = phoneNumber;
    }
}