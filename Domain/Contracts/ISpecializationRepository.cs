using policlinicApp.Domain.Entities;

namespace policlinicApp.Domain.Entities;

public interface ISpecializationRepository
{
    Specialization GetById(int id);
    IEnumerable<Specialization> GetAll();
    Specialization GetByName(string name);
    void Add(Specialization specialization);
    void Update(Specialization specialization);
    void Delete(int id);
}