using Domain.Entities;
using Domain.Enums;

namespace Domain.TestData;

public static class TestData
{
    public static List<Patient> Patients { get; } = new()
    {
        new Patient { PassportNumber = "AB100001", FullName = "Иванов Иван Иванович", Gender = Gender.Male, 
                     BirthDate = new DateTime(1980, 5, 15), Address = "ул. Ленина, д. 1", 
                     BloodGroup = BloodGroup.A, RhFactor = RhFactor.Positive, PhoneNumber = "+7 (915) 111-2233" },
        new Patient { PassportNumber = "AB100002", FullName = "Петрова Ольга Сергеевна", Gender = Gender.Female, 
                     BirthDate = new DateTime(1975, 8, 20), Address = "ул. Мира, д. 25", 
                     BloodGroup = BloodGroup.B, RhFactor = RhFactor.Negative, PhoneNumber = "+7 (916) 222-3344" },
        new Patient { PassportNumber = "AB100003", FullName = "Сидоров Алексей Петрович", Gender = Gender.Male, 
                     BirthDate = new DateTime(1990, 3, 10), Address = "ул. Советская, д. 15", 
                     BloodGroup = BloodGroup.O, RhFactor = RhFactor.Positive, PhoneNumber = "+7 (917) 333-4455" },
        new Patient { PassportNumber = "AB100004", FullName = "Козлова Мария Владимировна", Gender = Gender.Female, 
                     BirthDate = new DateTime(1985, 12, 5), Address = "ул. Центральная, д. 8", 
                     BloodGroup = BloodGroup.Ab, RhFactor = RhFactor.Positive, PhoneNumber = "+7 (918) 444-5566" },
        new Patient { PassportNumber = "AB100005", FullName = "Федоров Дмитрий Николаевич", Gender = Gender.Male, 
                     BirthDate = new DateTime(1970, 7, 30), Address = "ул. Школьная, д. 12", 
                     BloodGroup = BloodGroup.A, RhFactor = RhFactor.Negative, PhoneNumber = "+7 (919) 555-6677" },
        new Patient { PassportNumber = "AB100006", FullName = "Николаева Екатерина Андреевна", Gender = Gender.Female, 
                     BirthDate = new DateTime(1995, 1, 25), Address = "ул. Садовая, д. 3", 
                     BloodGroup = BloodGroup.B, RhFactor = RhFactor.Positive, PhoneNumber = "+7 (920) 666-7788" },
        new Patient { PassportNumber = "AB100007", FullName = "Волков Сергей Михайлович", Gender = Gender.Male, 
                     BirthDate = new DateTime(1988, 9, 14), Address = "ул. Парковая, д. 7", 
                     BloodGroup = BloodGroup.O, RhFactor = RhFactor.Negative, PhoneNumber = "+7 (921) 777-8899" },
        new Patient { PassportNumber = "AB100008", FullName = "Смирнова Анна Дмитриевна", Gender = Gender.Female, 
                     BirthDate = new DateTime(1978, 11, 8), Address = "ул. Лесная, д. 18", 
                     BloodGroup = BloodGroup.Ab, RhFactor = RhFactor.Negative, PhoneNumber = "+7 (922) 888-9900" },
        new Patient { PassportNumber = "AB100009", FullName = "Попов Андрей Викторович", Gender = Gender.Male, 
                     BirthDate = new DateTime(1983, 4, 17), Address = "ул. Новая, д. 22", 
                     BloodGroup = BloodGroup.A, RhFactor = RhFactor.Positive, PhoneNumber = "+7 (923) 999-0011" },
        new Patient { PassportNumber = "AB100010", FullName = "Васильева Татьяна Олеговна", Gender = Gender.Female, 
                     BirthDate = new DateTime(1992, 6, 12), Address = "ул. Заречная, д. 9", 
                     BloodGroup = BloodGroup.B, RhFactor = RhFactor.Positive, PhoneNumber = "+7 (924) 000-1122" }
    };

    public static List<Specialization> Specializations { get; } = new()
    {
        new Specialization { Id = 1, Name = "Терапевт", Description = "Врач общей практики" },
        new Specialization { Id = 2, Name = "Хирург", Description = "Хирургические операции" },
        new Specialization { Id = 3, Name = "Кардиолог", Description = "Лечение заболеваний сердца" },
        new Specialization { Id = 4, Name = "Невролог", Description = "Лечение нервной системы" },
        new Specialization { Id = 5, Name = "Офтальмолог", Description = "Лечение заболеваний глаз" },
        new Specialization { Id = 6, Name = "Стоматолог", Description = "Лечение зубов" },
        new Specialization { Id = 7, Name = "Дерматолог", Description = "Лечение кожи" },
        new Specialization { Id = 8, Name = "Гастроэнтеролог", Description = "Лечение ЖКТ" },
        new Specialization { Id = 9, Name = "Эндокринолог", Description = "Лечение гормонов" },
        new Specialization { Id = 10, Name = "Педиатр", Description = "Детский врач" }
    };

    public static List<Doctor> Doctors { get; } = new()
    {
        new Doctor { PassportNumber = "CD200001", FullName = "Врачев Александр Петрович", 
                    BirthYear = 1970, Specialization = Specializations[0], ExperienceYears = 25 },
        new Doctor { PassportNumber = "CD200002", FullName = "Докторова Елена Владимировна", 
                    BirthYear = 1980, Specialization = Specializations[1], ExperienceYears = 15 },
        new Doctor { PassportNumber = "CD200003", FullName = "Лекарев Сергей Иванович", 
                    BirthYear = 1965, Specialization = Specializations[2], ExperienceYears = 30 },
        new Doctor { PassportNumber = "CD200004", FullName = "Медицинская Ольга Сергеевна", 
                    BirthYear = 1975, Specialization = Specializations[3], ExperienceYears = 20 },
        new Doctor { PassportNumber = "CD200005", FullName = "Хирургов Дмитрий Николаевич", 
                    BirthYear = 1985, Specialization = Specializations[1], ExperienceYears = 8 },
        new Doctor { PassportNumber = "CD200006", FullName = "Терапевтова Ирина Алексеевна", 
                    BirthYear = 1978, Specialization = Specializations[0], ExperienceYears = 17 },
        new Doctor { PassportNumber = "CD200007", FullName = "Кардиологов Михаил Викторович", 
                    BirthYear = 1968, Specialization = Specializations[2], ExperienceYears = 27 },
        new Doctor { PassportNumber = "CD200008", FullName = "Невролова Светлана Дмитриевна", 
                    BirthYear = 1982, Specialization = Specializations[3], ExperienceYears = 12 },
        new Doctor { PassportNumber = "CD200009", FullName = "Офтальмологов Андрей Сергеевич", 
                    BirthYear = 1973, Specialization = Specializations[4], ExperienceYears = 22 },
        new Doctor { PassportNumber = "CD200010", FullName = "Стоматолова Мария Петровна", 
                    BirthYear = 1987, Specialization = Specializations[0], ExperienceYears = 6 }
    };

    public static List<Appointment> Appointments { get; } = new()
    {
        // Пациент 0 (Иванов) имеет 2 записи к разным врачам
        new Appointment { Id = 1, Patient = Patients[0], Doctor = Doctors[0], 
                         AppointmentDateTime = new DateTime(2024, 1, 15, 10, 0, 0), 
                         RoomNumber = "101", IsFollowUp = false },
        new Appointment { Id = 2, Patient = Patients[0], Doctor = Doctors[1], 
                         AppointmentDateTime = new DateTime(2024, 1, 20, 14, 0, 0), 
                         RoomNumber = "102", IsFollowUp = true },

        // Пациент 1 (Петрова) имеет 2 записи к разным врачам  
        new Appointment { Id = 3, Patient = Patients[1], Doctor = Doctors[2], 
                         AppointmentDateTime = new DateTime(2024, 1, 16, 9, 0, 0), 
                         RoomNumber = "103", IsFollowUp = false },
        new Appointment { Id = 4, Patient = Patients[1], Doctor = Doctors[3], 
                         AppointmentDateTime = new DateTime(2024, 1, 25, 11, 0, 0), 
                         RoomNumber = "104", IsFollowUp = false },

        // Повторные приемы за последний месяц - должно быть 2
        new Appointment { Id = 5, Patient = Patients[2], Doctor = Doctors[0], 
                         AppointmentDateTime = DateTime.Now.AddMonths(-1).AddDays(-5), 
                         RoomNumber = "105", IsFollowUp = true },
        new Appointment { Id = 6, Patient = Patients[3], Doctor = Doctors[1], 
                         AppointmentDateTime = DateTime.Now.AddMonths(-1).AddDays(-10), 
                         RoomNumber = "106", IsFollowUp = true },

        // Приемы в кабинете 101 за текущий месяц
        new Appointment { Id = 7, Patient = Patients[4], Doctor = Doctors[2], 
                         AppointmentDateTime = DateTime.Now.AddDays(-2), 
                         RoomNumber = "101", IsFollowUp = false },
        new Appointment { Id = 8, Patient = Patients[0], Doctor = Doctors[3], 
                         AppointmentDateTime = DateTime.Now.AddDays(-5), 
                         RoomNumber = "101", IsFollowUp = true },

        // Еще записи для полноты данных
        new Appointment { Id = 9, Patient = Patients[2], Doctor = Doctors[4], 
                         AppointmentDateTime = new DateTime(2024, 1, 18, 16, 0, 0), 
                         RoomNumber = "107", IsFollowUp = false },
        new Appointment { Id = 10, Patient = Patients[3], Doctor = Doctors[0], 
                         AppointmentDateTime = new DateTime(2024, 1, 22, 13, 0, 0), 
                         RoomNumber = "108", IsFollowUp = false }
    };
}