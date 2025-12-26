using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Data;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DataSeeder> _logger;
    private readonly Random _random = new();

    public DataSeeder(ApplicationDbContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("🌱 Starting database seeding...");
        
        try
        {
            if (await _context.Patients.AnyAsync())
            {
                _logger.LogInformation("Database already contains data, skipping seeding.");
                return;
            }

            await SeedSpecializationsAsync();
            await SeedDoctorsAsync();
            await SeedPatientsAsync();
            await SeedAppointmentsAsync();
            
            _logger.LogInformation("✅ Database seeded successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error seeding database");
        }
    }

    private async Task SeedSpecializationsAsync()
    {
        var specializations = new List<Specialization>
        {
            new() { Name = "Терапевт" },
            new() { Name = "Хирург" },
            new() { Name = "Кардиолог" },
            new() { Name = "Невролог" },
            new() { Name = "Офтальмолог" },
            new() { Name = "Отоларинголог" },
            new() { Name = "Стоматолог" },
            new() { Name = "Педиатр" }
        };

        await _context.Specializations.AddRangeAsync(specializations);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"✅ Added {specializations.Count} specializations");
    }

    private async Task SeedDoctorsAsync()
    {
        var specializations = await _context.Specializations.ToListAsync();
        
        var doctors = new List<Doctor>
        {
            new()
            {
                PassportNumber = "4501 123456",
                FullName = "Иванов Иван Иванович",
                BirthYear = 1975,
                SpecializationId = specializations[0].Id, // Терапевт
                ExperienceYears = 15
            },
            new()
            {
                PassportNumber = "4602 234567",
                FullName = "Петрова Анна Сергеевна",
                BirthYear = 1980,
                SpecializationId = specializations[1].Id, // Хирург
                ExperienceYears = 10
            },
            new()
            {
                PassportNumber = "4503 345678",
                FullName = "Сидоров Дмитрий Петрович",
                BirthYear = 1978,
                SpecializationId = specializations[2].Id, // Кардиолог
                ExperienceYears = 12
            },
            new()
            {
                PassportNumber = "4604 456789",
                FullName = "Кузнецова Елена Владимировна",
                BirthYear = 1985,
                SpecializationId = specializations[3].Id, // Невролог
                ExperienceYears = 8
            },
            new()
            {
                PassportNumber = "4405 567890",
                FullName = "Морозов Андрей Алексеевич",
                BirthYear = 1970,
                SpecializationId = specializations[4].Id, // Офтальмолог
                ExperienceYears = 20
            },
            new()
            {
                PassportNumber = "4506 678901",
                FullName = "Николаева Ольга Дмитриевна",
                BirthYear = 1982,
                SpecializationId = specializations[5].Id, // Отоларинголог
                ExperienceYears = 11
            },
            new()
            {
                PassportNumber = "4607 789012",
                FullName = "Григорьев Сергей Викторович",
                BirthYear = 1973,
                SpecializationId = specializations[6].Id, // Стоматолог
                ExperienceYears = 17
            },
            new()
            {
                PassportNumber = "4508 890123",
                FullName = "Васильева Татьяна Михайловна",
                BirthYear = 1988,
                SpecializationId = specializations[7].Id, // Педиатр
                ExperienceYears = 7
            }
        };

        await _context.Doctors.AddRangeAsync(doctors);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"✅ Added {doctors.Count} doctors");
    }

    private async Task SeedPatientsAsync()
    {
        var patients = new List<Patient>
        {
            new()
            {
                PassportNumber = "4510 123456",
                FullName = "Смирнов Алексей Дмитриевич",
                Gender = Gender.Male,
                BirthDate = new DateOnly(1990, 5, 15),
                Address = "г. Москва, ул. Ленина, д. 10, кв. 5",
                BloodGroup = BloodGroup.A,
                RhFactor = RhFactor.Pos,
                PhoneNumber = "+7 (911) 111-11-11"
            },
            new()
            {
                PassportNumber = "4520 234567",
                FullName = "Козлова Мария Ивановна",
                Gender = Gender.Female,
                BirthDate = new DateOnly(1985, 8, 22),
                Address = "г. Москва, ул. Пушкина, д. 25, кв. 12",
                BloodGroup = BloodGroup.O,
                RhFactor = RhFactor.Pos,
                PhoneNumber = "+7 (911) 222-22-22"
            },
            new()
            {
                PassportNumber = "4530 345678",
                FullName = "Новиков Дмитрий Сергеевич",
                Gender = Gender.Male,
                BirthDate = new DateOnly(1978, 3, 10),
                Address = "г. Москва, пр. Мира, д. 45, кв. 8",
                BloodGroup = BloodGroup.B,
                RhFactor = RhFactor.Neg,
                PhoneNumber = "+7 (911) 333-33-33"
            },
            new()
            {
                PassportNumber = "4540 456789",
                FullName = "Федорова Елена Владимировна",
                Gender = Gender.Female,
                BirthDate = new DateOnly(1995, 11, 30),
                Address = "г. Москва, ул. Садовая, д. 15, кв. 3",
                BloodGroup = BloodGroup.Ab,
                RhFactor = RhFactor.Pos,
                PhoneNumber = "+7 (911) 444-44-44"
            },
            new()
            {
                PassportNumber = "4550 567890",
                FullName = "Волков Петр Николаевич",
                Gender = Gender.Male,
                BirthDate = new DateOnly(1982, 7, 14),
                Address = "г. Москва, ул. Лесная, д. 8, кв. 21",
                BloodGroup = BloodGroup.A,
                RhFactor = RhFactor.Neg,
                PhoneNumber = "+7 (911) 555-55-55"
            },
            new()
            {
                PassportNumber = "4560 678901",
                FullName = "Захарова Ольга Александровна",
                Gender = Gender.Female,
                BirthDate = new DateOnly(1992, 2, 28),
                Address = "г. Москва, ул. Цветочная, д. 33, кв. 7",
                BloodGroup = BloodGroup.O,
                RhFactor = RhFactor.Pos,
                PhoneNumber = "+7 (911) 666-66-66"
            },
            new()
            {
                PassportNumber = "4570 789012",
                FullName = "Белов Игорь Викторович",
                Gender = Gender.Male,
                BirthDate = new DateOnly(1988, 9, 5),
                Address = "г. Москва, ул. Солнечная, д. 12, кв. 15",
                BloodGroup = BloodGroup.B,
                RhFactor = RhFactor.Pos,
                PhoneNumber = "+7 (911) 777-77-77"
            },
            new()
            {
                PassportNumber = "4580 890123",
                FullName = "Григорьева Татьяна Михайловна",
                Gender = Gender.Female,
                BirthDate = new DateOnly(1975, 4, 18),
                Address = "г. Москва, ул. Речная, д. 7, кв. 9",
                BloodGroup = BloodGroup.A,
                RhFactor = RhFactor.Pos,
                PhoneNumber = "+7 (911) 888-88-88"
            },
            new()
            {
                PassportNumber = "4590 901234",
                FullName = "Денисов Роман Олегович",
                Gender = Gender.Male,
                BirthDate = new DateOnly(1998, 12, 3),
                Address = "г. Москва, ул. Горная, д. 19, кв. 4",
                BloodGroup = BloodGroup.Ab,
                RhFactor = RhFactor.Neg,
                PhoneNumber = "+7 (911) 999-99-99"
            },
            new()
            {
                PassportNumber = "4600 012345",
                FullName = "Ершова Наталья Павловна",
                Gender = Gender.Female,
                BirthDate = new DateOnly(1980, 6, 25),
                Address = "г. Москва, ул. Парковая, д. 22, кв. 11",
                BloodGroup = BloodGroup.O,
                RhFactor = RhFactor.Pos,
                PhoneNumber = "+7 (911) 000-00-00"
            }
        };

        await _context.Patients.AddRangeAsync(patients);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"✅ Added {patients.Count} patients");
    }

    private async Task SeedAppointmentsAsync()
    {
        var patients = await _context.Patients.Take(10).ToListAsync();
        var doctors = await _context.Doctors.Take(8).ToListAsync();
        
        if (!patients.Any() || !doctors.Any())
        {
            _logger.LogWarning("Cannot seed appointments: no patients or doctors");
            return;
        }

        var appointments = new List<Appointment>();
        
        // Номера кабинетов
        var roomNumbers = new[] { "101", "102", "103", "104", "105", "201", "202", "203", "204", "301" };
        
        // Создаем 30 записей на прием
        for (var i = 1; i <= 30; i++)
        {
            // DateTime вместо DateOnly
            var appointmentDateTime = DateTime.Now
                .AddDays(_random.Next(-30, 30)) // Дата ±30 дней от сегодня
                .AddHours(_random.Next(9, 18))  // Время с 9:00 до 18:00
                .AddMinutes(_random.Next(0, 12) * 5); // Кратно 5 минутам (0, 5, 10, ... 55)
            
            // Округляем минуты
            appointmentDateTime = new DateTime(
                appointmentDateTime.Year,
                appointmentDateTime.Month,
                appointmentDateTime.Day,
                appointmentDateTime.Hour,
                appointmentDateTime.Minute - (appointmentDateTime.Minute % 5),
                0
            );
            
            var appointment = new Appointment
            {
                PatientId = patients[_random.Next(patients.Count)].Id,
                DoctorId = doctors[_random.Next(doctors.Count)].Id,
                AppointmentDateTime = appointmentDateTime, // DateTime, а не DateOnly
                RoomNumber = roomNumbers[_random.Next(roomNumbers.Length)],
                IsFollowUp = _random.Next(100) < 30 // 30% вероятность что повторный прием
            };
            
            appointments.Add(appointment);
        }

        await _context.Appointments.AddRangeAsync(appointments);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"✅ Added {appointments.Count} appointments");
    }
}