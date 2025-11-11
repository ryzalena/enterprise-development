using Domain.Entities;
using Domain.Enums;

namespace Domain.TestData;

/// <summary>
/// Статические тестовые данные для поликлиники
/// </summary>
public static class TestData
{
    /// <summary>
    /// Список пациентов для тестирования
    /// </summary>
    public static List<Patient> Patients { get; } =
    [
        new Patient 
        { 
            Id = 1,
            PassportNumber = "AB100001", 
            FullName = "Иванов Иван Иванович", 
            Gender = Gender.Male, 
            BirthDate = new DateOnly(1980, 5, 15), 
            Address = "ул. Ленина, д. 1", 
            BloodGroup = BloodGroup.A, 
            RhFactor = RhFactor.Positive, 
            PhoneNumber = "+7 (915) 111-2233" 
        },
        new Patient 
        { 
            Id = 2,
            PassportNumber = "AB100002", 
            FullName = "Петрова Ольга Сергеевна", 
            Gender = Gender.Female, 
            BirthDate = new DateOnly(1975, 8, 20), 
            Address = "ул. Мира, д. 25", 
            BloodGroup = BloodGroup.B, 
            RhFactor = RhFactor.Negative, 
            PhoneNumber = "+7 (916) 222-3344" 
        },
        new Patient 
        { 
            Id = 3,
            PassportNumber = "AB100003", 
            FullName = "Сидоров Алексей Петрович", 
            Gender = Gender.Male, 
            BirthDate = new DateOnly(1990, 3, 10), 
            Address = "ул. Советская, д. 15", 
            BloodGroup = BloodGroup.O, 
            RhFactor = RhFactor.Positive, 
            PhoneNumber = "+7 (917) 333-4455" 
        },
        new Patient 
        { 
            Id = 4,
            PassportNumber = "AB100004", 
            FullName = "Козлова Мария Владимировна", 
            Gender = Gender.Female, 
            BirthDate = new DateOnly(1985, 12, 5), 
            Address = "ул. Центральная, д. 8", 
            BloodGroup = BloodGroup.Ab, 
            RhFactor = RhFactor.Positive, 
            PhoneNumber = "+7 (918) 444-5566" 
        },
        new Patient 
        { 
            Id = 5,
            PassportNumber = "AB100005", 
            FullName = "Федоров Дмитрий Николаевич", 
            Gender = Gender.Male, 
            BirthDate = new DateOnly(1970, 7, 30), 
            Address = "ул. Школьная, д. 12", 
            BloodGroup = BloodGroup.A, 
            RhFactor = RhFactor.Negative, 
            PhoneNumber = "+7 (919) 555-6677" 
        }
    ];

    /// <summary>
    /// Список специализаций врачей
    /// </summary>
    public static List<Specialization> Specializations { get; } =
    [
        new Specialization 
        { 
            Id = 1, 
            Name = "Терапевт", 
            Description = "Врач общей практики" 
        },
        new Specialization 
        { 
            Id = 2, 
            Name = "Хирург", 
            Description = "Хирургические операции" 
        },
        new Specialization 
        { 
            Id = 3, 
            Name = "Кардиолог", 
            Description = "Лечение заболеваний сердца" 
        },
        new Specialization 
        { 
            Id = 4, 
            Name = "Невролог", 
            Description = "Лечение нервной системы" 
        },
        new Specialization 
        { 
            Id = 5, 
            Name = "Офтальмолог", 
            Description = "Лечение заболеваний глаз" 
        },
        new Specialization
        {
            Id = 6, 
            Name = "Стоматолог", 
            Description = "Лечение зубов"
        },
        new Specialization
        {
            Id = 7, 
            Name = "Дерматолог", 
            Description = "Лечение кожи"
        },
        new Specialization
        {
            Id = 8, 
            Name = "Гастроэнтеролог", 
            Description = "Лечение ЖКТ"
        },
        new Specialization
        {
            Id = 9, 
            Name = "Эндокринолог", 
            Description = "Лечение гормонов"
        },
        new Specialization
        {
            Id = 10, 
            Name = "Педиатр", 
            Description = "Детский врач"
        }
    ];

    /// <summary>
    /// Список врачей поликлиники
    /// </summary>
    public static List<Doctor> Doctors { get; } =
    [
        new Doctor 
        { 
            Id = 1,
            PassportNumber = "CD200001", 
            FullName = "Врачев Александр Петрович", 
            BirthYear = 1970, 
            Specialization = Specializations[0], 
            ExperienceYears = 25 
        },
        new Doctor 
        { 
            Id = 2,
            PassportNumber = "CD200002", 
            FullName = "Докторова Елена Владимировна", 
            BirthYear = 1980, 
            Specialization = Specializations[1], 
            ExperienceYears = 15 
        },
        new Doctor 
        { 
            Id = 3,
            PassportNumber = "CD200003", 
            FullName = "Лекарев Сергей Иванович", 
            BirthYear = 1965, 
            Specialization = Specializations[2], 
            ExperienceYears = 30 
        },
        new Doctor 
        { 
            Id = 4,
            PassportNumber = "CD200004", 
            FullName = "Медицинская Ольга Сергеевна", 
            BirthYear = 1975, 
            Specialization = Specializations[3], 
            ExperienceYears = 20 
        },
        new Doctor 
        { 
            Id = 5,
            PassportNumber = "CD200005", 
            FullName = "Хирургов Дмитрий Николаевич", 
            BirthYear = 1985, 
            Specialization = Specializations[1], 
            ExperienceYears = 8 
        }
    ];

    /// <summary>
    /// Список записей на прием
    /// </summary>
    public static List<Appointment> Appointments { get; } =
    [
        new Appointment 
        { 
            Id = 1, 
            PatientId = 1, 
            Patient = Patients[0], 
            DoctorId = 1, 
            Doctor = Doctors[0], 
            AppointmentDateTime = new DateTime(2024, 1, 15, 10, 0, 0), 
            RoomNumber = "101", 
            IsFollowUp = false 
        },
        new Appointment 
        { 
            Id = 2, 
            PatientId = 1, 
            Patient = Patients[0], 
            DoctorId = 2, 
            Doctor = Doctors[1], 
            AppointmentDateTime = new DateTime(2024, 1, 20, 14, 0, 0), 
            RoomNumber = "102", 
            IsFollowUp = true 
        },
        new Appointment 
        { 
            Id = 3, 
            PatientId = 2, 
            Patient = Patients[1], 
            DoctorId = 3, 
            Doctor = Doctors[2], 
            AppointmentDateTime = new DateTime(2024, 1, 16, 9, 0, 0), 
            RoomNumber = "103", 
            IsFollowUp = false 
        },
        new Appointment 
        { 
            Id = 4, 
            PatientId = 2, 
            Patient = Patients[1], 
            DoctorId = 4, 
            Doctor = Doctors[3], 
            AppointmentDateTime = new DateTime(2024, 1, 25, 11, 0, 0), 
            RoomNumber = "104", 
            IsFollowUp = false 
        },
        new Appointment 
        { 
            Id = 5, 
            PatientId = 3, 
            Patient = Patients[2], 
            DoctorId = 1, 
            Doctor = Doctors[0], 
            AppointmentDateTime = new DateTime(2023, 12, 20, 10, 0, 0),
            RoomNumber = "105", 
            IsFollowUp = true 
        },
        new Appointment 
        { 
            Id = 6, 
            PatientId = 4, 
            Patient = Patients[3], 
            DoctorId = 2, 
            Doctor = Doctors[1], 
            AppointmentDateTime = new DateTime(2023, 12, 15, 14, 0, 0),
            RoomNumber = "106", 
            IsFollowUp = true 
        },
        new Appointment 
        { 
            Id = 7, 
            PatientId = 5, 
            Patient = Patients[4], 
            DoctorId = 3, 
            Doctor = Doctors[2], 
            AppointmentDateTime = new DateTime(2024, 1, 13, 9, 0, 0),
            RoomNumber = "101", 
            IsFollowUp = false 
        },
        new Appointment 
        { 
            Id = 8, 
            PatientId = 1, 
            Patient = Patients[0], 
            DoctorId = 4, 
            Doctor = Doctors[3], 
            AppointmentDateTime = new DateTime(2024, 1, 10, 11, 0, 0),
            RoomNumber = "101", 
            IsFollowUp = true 
        }
    ];
}