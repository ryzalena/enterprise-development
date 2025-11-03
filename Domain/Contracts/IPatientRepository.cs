using System.Collections.Generic;
using policlinicApp.Domain.Entities;
using policlinicApp.Domain.Enums;

namespace policlinicApp.Domain.Entities;

public interface IPatientRepository
{
    Patient GetByPassport(string passportNumber);
    IEnumerable<Patient> GetAll();
    IEnumerable<Patient> GetPatientsOverAge(int age);
    IEnumerable<Patient> GetPatientsByBloodGroup(BloodGroup bloodGroup);
    void Add(Patient patient);
    void Update(Patient patient);
    void Delete(string passportNumber);
}