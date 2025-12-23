using Bogus;
using DataGenerator.Models;

namespace DataGenerator.Services
{
    public interface IContractGenerator
    {
        List<ContractMessage> GenerateContracts(int count, string generatorId);
    }

    public class ContractGenerator : IContractGenerator
    {
        private readonly Faker<ContractMessage> _contractFaker;

        public ContractGenerator()
        {
            // Инициализация генератора данных для поликлиники
            var diagnoses = new[]
            {
                "ОРВИ", "Гипертоническая болезнь", "Гастрит", 
                "Остеохондроз", "Бронхит", "Сахарный диабет"
            };
            
            var services = new[]
            {
                "Консультация", "Диагностика", "Лечение",
                "Реабилитация", "Профилактический осмотр"
            };
            
            var medications = new[]
            {
                "Парацетамол", "Ибупрофен", "Амоксициллин",
                "Лоратадин", "Метформин", "Омепразол"
            };

            _contractFaker = new Faker<ContractMessage>()
                .RuleFor(c => c.Id, f => $"CONTRACT-{DateTime.Now:yyyyMMdd}-{f.Random.Number(1000, 9999)}")
                .RuleFor(c => c.PatientId, f => $"PAT-{f.Random.Number(1000, 9999)}")
                .RuleFor(c => c.DoctorId, f => $"DOC-{f.Random.Number(100, 999)}")
                .RuleFor(c => c.ServiceType, f => f.PickRandom(services))
                .RuleFor(c => c.Price, f => f.Random.Decimal(500, 10000))
                .RuleFor(c => c.Status, f => f.PickRandom("Создан", "Ожидает оплаты", "Оплачен"))
                .RuleFor(c => c.CreatedDate, f => f.Date.Recent(30))
                .RuleFor(c => c.ValidUntil, (f, c) => c.CreatedDate.AddDays(f.Random.Number(7, 90)))
                .RuleFor(c => c.AppointmentId, f => $"APT-{f.Random.Number(10000, 99999)}")
                .RuleFor(c => c.Diagnosis, f => f.PickRandom(diagnoses))
                .RuleFor(c => c.PrescribedMedications, f => 
                    f.Make(f.Random.Number(1, 3), () => f.PickRandom(medications)).ToList())
                .RuleFor(c => c.TreatmentPlan, f => f.Lorem.Sentence(5))
                .RuleFor(c => c.Timestamp, f => DateTime.UtcNow);
        }

        public List<ContractMessage> GenerateContracts(int count, string generatorId)
        {
            return _contractFaker.Generate(count).Select(c =>
            {
                c.GeneratorId = generatorId;
                return c;
            }).ToList();
        }
    }
}