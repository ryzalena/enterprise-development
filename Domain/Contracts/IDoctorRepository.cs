using policlinicApp.Domain.Entities;

namespace policlinicApp.Domain.Entities;

public interface IDoctorRepository
{
    Doctor GetByPassport(string passportNumber);
    IEnumerable<Doctor> GetAll();
    IEnumerable<Doctor> GetDoctorsBySpecialization(int specializationId);
    IEnumerable<Doctor> GetDoctorsWithExperience(int minExperienceYears);
    void Add(Doctor doctor);
    void Update(Doctor doctor);
    void Delete(string passportNumber);
}