using System;
using System.Collections.Generic;
using System.Linq;
using policlinicApp.Domain.Entities;
using policlinicApp.Domain.Enums;

namespace PolyclinicApp.Data.Seeders;

internal static class TestDataGenerator
{
    private static readonly Random _random = new Random();
    private static int _appointmentId = 1;

    public static (List<Patient>, List<Doctor>, List<Appointment>, List<Specialization>) GenerateTestData()
    {
        var specializations = GenerateSpecializations();
        var patients = GeneratePatients();
        var doctors = GenerateDoctors(specializations);
        var appointments = GenerateAppointments(patients, doctors);

        return (patients, doctors, appointments, specializations);
    }

    private static List<Specialization> GenerateSpecializations()
    {
        return new List<Specialization>
        {
            new Specialization(1, "Терапевт", "Врач общей практики"),
            new Specialization(2, "Хирург", "Хирургические операции"),
            new Specialization(3, "Кардиолог", "Лечение заболеваний сердца"),
            new Specialization(4, "Невролог", "Лечение нервной системы"),
            new Specialization(5, "Офтальмолог", "Лечение заболеваний глаз"),
            new Specialization(6, "Отоларинголог", "Лечение уха, горла, носа"),
            new Specialization(7, "Стоматолог", "Лечение зубов и полости рта"),
            new Specialization(8, "Педиатр", "Детский врач"),
            new Specialization(9, "Гинеколог", "Женское здоровье"),
            new Specialization(10, "Уролог", "Мочеполовая система"),
            new Specialization(11, "Дерматолог", "Кожные заболевания"),
            new Specialization(12, "Эндокринолог", "Лечение эндокринной системы"),
            new Specialization(13, "Гастроэнтеролог", "Лечение ЖКТ"),
            new Specialization(14, "Ортопед", "Опорно-двигательный аппарат"),
            new Specialization(15, "Психиатр", "Психическое здоровье")
        };
    }

    private static List<Patient> GeneratePatients()
    {
        var patients = new List<Patient>();
        
        // Мужские имена 
        var maleFirstNames = new[] { "Иван", "Петр", "Сергей", "Алексей", "Дмитрий", "Андрей", "Михаил", "Владимир", "Николай", "Евгений" };
        var maleLastNames = new[] { "Иванов", "Петров", "Сидоров", "Смирнов", "Кузнецов", "Попов", "Васильев", "Федоров", "Морозов", "Волков" };
        var maleMiddleNames = new[] { "Иванович", "Петрович", "Сергеевич", "Алексеевич", "Дмитриевич", "Андреевич", "Михайлович", "Владимирович", "Николаевич", "Евгеньевич" };

        // Женские имена 
        var femaleFirstNames = new[] { "Елена", "Ольга", "Ирина", "Наталья", "Мария", "Светлана", "Анна", "Татьяна", "Юлия", "Екатерина" };
        var femaleLastNames = new[] { "Иванова", "Петрова", "Сидорова", "Смирнова", "Кузнецова", "Попова", "Васильева", "Федорова", "Морозова", "Волкова" };
        var femaleMiddleNames = new[] { "Ивановна", "Петровна", "Сергеевна", "Алексеевна", "Дмитриевна", "Андреевна", "Михайловна", "Владимировна", "Николаевна", "Евгеньевна" };

        // Генерируем 15 пациентов
        for (var i = 1; i <= 15; i++)
        {
            var isMale = i % 2 == 1;
            string firstName, lastName, middleName, fullName;

            if (isMale)
            {
                firstName = maleFirstNames[_random.Next(maleFirstNames.Length)];
                lastName = maleLastNames[_random.Next(maleLastNames.Length)];
                middleName = maleMiddleNames[_random.Next(maleMiddleNames.Length)];
                fullName = $"{lastName} {firstName} {middleName}";
            }
            else
            {
                firstName = femaleFirstNames[_random.Next(femaleFirstNames.Length)];
                lastName = femaleLastNames[_random.Next(femaleLastNames.Length)];
                middleName = femaleMiddleNames[_random.Next(femaleMiddleNames.Length)];
                fullName = $"{lastName} {firstName} {middleName}";
            }

            var birthDate = new DateTime(1950 + _random.Next(40), _random.Next(1, 13), _random.Next(1, 28));
            var bloodGroup = (BloodGroup)_random.Next(0, 4);
            var rhFactor = (RhFactor)_random.Next(0, 2);

            patients.Add(new Patient(
                passportNumber: $"AB{100000 + i:D6}",
                fullName: fullName,
                gender: isMale ? Gender.Male : Gender.Female,
                birthDate: birthDate,
                address: $"ул. Поликлиническая, д. {i}, кв. {_random.Next(1, 100)}",
                bloodGroup: bloodGroup,
                rhFactor: rhFactor,
                phoneNumber: $"+7{9000000000 + i:D10}"
            ));
        }

        return patients;
    }

    private static List<Doctor> GenerateDoctors(List<Specialization> specializations)
    {
        var doctors = new List<Doctor>();
        
        var maleFirstNames = new[] { "Александр", "Владимир", "Сергей", "Андрей", "Дмитрий", "Михаил", "Евгений", "Алексей", "Игорь", "Юрий" };
        var maleLastNames = new[] { "Врачев", "Докторов", "Лекарев", "Медицинский", "Хирургов", "Терапевтов", "Кардиологов", "Невролов", "Стоматолов", "Педиатров" };
        var maleMiddleNames = new[] { "Александрович", "Владимирович", "Сергеевич", "Андреевич", "Дмитриевич", "Михайлович", "Евгеньевич", "Алексеевич", "Игоревич", "Юрьевич" };

        var femaleFirstNames = new[] { "Елена", "Ольга", "Татьяна", "Наталья", "Ирина", "Мария", "Светлана", "Анна", "Юлия", "Екатерина" };
        var femaleLastNames = new[] { "Врачева", "Докторова", "Лекарева", "Медицинская", "Хирургова", "Терапевтова", "Кардиологова", "Невролова", "Стоматолова", "Педиатрова" };
        var femaleMiddleNames = new[] { "Александровна", "Владимировна", "Сергеевна", "Андреевна", "Дмитриевна", "Михайловна", "Евгеньевна", "Алексеевна", "Игоревна", "Юрьевна" };

        // Генерируем 12 врачей
        for (var i = 1; i <= 12; i++)
        {
            var isMale = i % 2 == 1;
            string firstName, lastName, middleName, fullName;

            if (isMale)
            {
                firstName = maleFirstNames[_random.Next(maleFirstNames.Length)];
                lastName = maleLastNames[_random.Next(maleLastNames.Length)];
                middleName = maleMiddleNames[_random.Next(maleMiddleNames.Length)];
                fullName = $"{lastName} {firstName} {middleName}";
            }
            else
            {
                firstName = femaleFirstNames[_random.Next(femaleFirstNames.Length)];
                lastName = femaleLastNames[_random.Next(femaleLastNames.Length)];
                middleName = femaleMiddleNames[_random.Next(femaleMiddleNames.Length)];
                fullName = $"{lastName} {firstName} {middleName}";
            }

            var birthYear = 1960 + _random.Next(25);
            var specialization = specializations[_random.Next(specializations.Count)];
            var experienceYears = _random.Next(5, 35);

            doctors.Add(new Doctor(
                passportNumber: $"CD{200000 + i:D6}",
                fullName: fullName,
                birthYear: birthYear,
                specialization: specialization,
                experienceYears: experienceYears
            ));
        }

        return doctors;
    }

    private static List<Appointment> GenerateAppointments(List<Patient> patients, List<Doctor> doctors)
    {
        var appointments = new List<Appointment>();
        _appointmentId = 1;

        // Базовые записи на прием
        for (var i = 0; i < 25; i++)
        {
            var patient = patients[_random.Next(patients.Count)];
            var doctor = doctors[_random.Next(doctors.Count)];
            var isFollowUp = _random.Next(0, 3) == 1; // 33% повторных приемов

            // Записи в разные месяцы для тестирования
            var baseDate = DateTime.Now.AddMonths(-1);
            var appointmentDate = baseDate.AddDays(_random.Next(0, 60))
                                        .AddHours(_random.Next(8, 18))
                                        .AddMinutes(_random.Next(0, 60));

            appointments.Add(new Appointment(
                id: _appointmentId++,
                patient: patient,
                doctor: doctor,
                appointmentDateTime: appointmentDate,
                roomNumber: $"10{_random.Next(1, 21):D2}",
                isFollowUp: isFollowUp
            ));
        }

        // Специальные данные для теста "пациенты старше 30 лет с несколькими врачами"
        var patientsOver30 = patients.Where(p => p.Age > 30).Take(4).ToList();
        foreach (var patient in patientsOver30)
        {
            var differentDoctors = doctors.Take(3).ToList(); // Берем 3 разных врачей
            foreach (var doctor in differentDoctors)
            {
                var appointmentDate = DateTime.Now.AddDays(_random.Next(1, 30))
                                                .AddHours(_random.Next(8, 18))
                                                .AddMinutes(_random.Next(0, 60));

                appointments.Add(new Appointment(
                    id: _appointmentId++,
                    patient: patient,
                    doctor: doctor,
                    appointmentDateTime: appointmentDate,
                    roomNumber: $"20{_random.Next(1, 6):D2}",
                    isFollowUp: false
                ));
            }
        }

        // Гарантируем несколько записей для конкретного врача 
        var specificDoctor = doctors[0];
        for (var i = 0; i < 5; i++)
        {
            var patient = patients[_random.Next(patients.Count)];
            var appointmentDate = DateTime.Now.AddDays(_random.Next(1, 30))
                                            .AddHours(_random.Next(8, 18))
                                            .AddMinutes(_random.Next(0, 60));

            appointments.Add(new Appointment(
                id: _appointmentId++,
                patient: patient,
                doctor: specificDoctor,
                appointmentDateTime: appointmentDate,
                roomNumber: $"30{_random.Next(1, 6):D2}",
                isFollowUp: _random.Next(0, 2) == 1
            ));
        }

        // Гарантируем несколько записей в конкретном кабинете за текущий месяц
        var targetRoom = "101";
        for (var i = 0; i < 4; i++)
        {
            var patient = patients[_random.Next(patients.Count)];
            var doctor = doctors[_random.Next(doctors.Count)];
            var appointmentDate = DateTime.Now.AddDays(_random.Next(0, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day))
                                            .AddHours(_random.Next(8, 18))
                                            .AddMinutes(_random.Next(0, 60));

            appointments.Add(new Appointment(
                id: _appointmentId++,
                patient: patient,
                doctor: doctor,
                appointmentDateTime: appointmentDate,
                roomNumber: targetRoom,
                isFollowUp: _random.Next(0, 2) == 1
            ));
        }

        // Гарантируем повторные приемы за последний месяц 
        var lastMonth = DateTime.Now.AddMonths(-1);
        var lastMonthDays = DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month);
        for (var i = 0; i < 6; i++)
        {
            var patient = patients[_random.Next(patients.Count)];
            var doctor = doctors[_random.Next(doctors.Count)];
            var appointmentDate = new DateTime(lastMonth.Year, lastMonth.Month, _random.Next(1, lastMonthDays + 1))
                                                .AddHours(_random.Next(8, 18))
                                                .AddMinutes(_random.Next(0, 60));

            appointments.Add(new Appointment(
                id: _appointmentId++,
                patient: patient,
                doctor: doctor,
                appointmentDateTime: appointmentDate,
                roomNumber: $"40{_random.Next(1, 6):D2}",
                isFollowUp: true // Гарантируем повторный прием
            ));
        }

        return appointments;
    }

    // Дополнительный метод для получения статистики по сгенерированным данным (для отладки)
    public static void PrintDataStatistics(List<Patient> patients, List<Doctor> doctors, List<Appointment> appointments, List<Specialization> specializations)
    {
        Console.WriteLine("=== СТАТИСТИКА ТЕСТОВЫХ ДАННЫХ ===");
        Console.WriteLine($"Пациенты: {patients.Count}");
        Console.WriteLine($"Врачи: {doctors.Count}");
        Console.WriteLine($"Записи на прием: {appointments.Count}");
        Console.WriteLine($"Специализации: {specializations.Count}");
        
        Console.WriteLine($"\nПациенты старше 30 лет: {patients.Count(p => p.Age > 30)}");
        Console.WriteLine($"Врачи со стажем ≥10 лет: {doctors.Count(d => d.ExperienceYears >= 10)}");
        
        var lastMonth = DateTime.Now.AddMonths(-1);
        var followUpLastMonth = appointments.Count(a => a.IsFollowUp && 
                                                      a.AppointmentDateTime.Month == lastMonth.Month && 
                                                      a.AppointmentDateTime.Year == lastMonth.Year);
        Console.WriteLine($"Повторные приемы за последний месяц: {followUpLastMonth}");
        
        var currentMonthAppointments = appointments.Count(a => 
            a.AppointmentDateTime.Month == DateTime.Now.Month && 
            a.AppointmentDateTime.Year == DateTime.Now.Year);
        Console.WriteLine($"Приемы за текущий месяц: {currentMonthAppointments}");
        
        var patientsWithMultipleDoctors = appointments
            .GroupBy(a => a.Patient)
            .Count(g => g.Select(a => a.Doctor.PassportNumber).Distinct().Count() > 1);
        Console.WriteLine($"Пациенты с несколькими врачами: {patientsWithMultipleDoctors}");
        
        Console.WriteLine("=== КОНЕЦ СТАТИСТИКИ ===\n");
    }
}
