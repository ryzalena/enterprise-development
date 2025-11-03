using System;
using System.Collections.Generic;
using System.Linq;
using policlinicApp.Domain.Entities;
using policlinicApp.Domain.Enums;
using policlinicApp.Services;
using policlinicApp.Services.Interfaces;
using PolyclinicApp.Data.Seeders;

namespace PolyclinicApp.Tests;

public class DatabaseTester
{
    public static void TestDatabase()
    {
        Console.WriteLine("=== ТЕСТИРОВАНИЕ БАЗЫ ДАННЫХ ПОЛИКЛИНИКИ ===\n");

        // Генерация тестовых данных
        var testData = TestDataGenerator.GenerateTestData();
        var patients = testData.Item1;
        var doctors = testData.Item2;
        var appointments = testData.Item3;
        var specializations = testData.Item4;

        // Создание сервиса
        var service = new PolyclinicService(patients, doctors, appointments, specializations);

        // 1. Базовая проверка данных
        TestBasicData(patients, doctors, appointments, specializations);

        // 2. Проверка связей между сущностями
        TestRelationships(patients, doctors, appointments);

        // 3. Проверка бизнес-логики
        TestBusinessLogic(service);

        // 4. Проверка целостности данных
        TestDataIntegrity(patients, doctors, appointments);

        // 5. Статистика базы данных
        PrintDatabaseStatistics(patients, doctors, appointments, specializations);

        Console.WriteLine("\n=== ТЕСТИРОВАНИЕ ЗАВЕРШЕНО ===");
    }

    private static void TestBasicData(List<Patient> patients, List<Doctor> doctors, 
                                    List<Appointment> appointments, List<Specialization> specializations)
    {
        Console.WriteLine("1. БАЗОВАЯ ПРОВЕРКА ДАННЫХ:");
        
        // Проверка пациентов
        Console.WriteLine($"   Пациенты: {patients.Count} записей");
        foreach (var patient in patients)
        {
            Console.WriteLine($"     - {patient.FullName}, {patient.Gender}, {patient.Age} лет, " +
                            $"{patient.BloodGroup}{patient.RhFactor}, тел: {patient.PhoneNumber}");
        }

        // Проверка врачей
        Console.WriteLine($"\n   Врачи: {doctors.Count} записей");
        foreach (var doctor in doctors)
        {
            Console.WriteLine($"     - {doctor.FullName}, {doctor.Specialization.Name}, " +
                            $"стаж: {doctor.ExperienceYears} лет, год рождения: {doctor.BirthYear}");
        }

        // Проверка специализаций
        Console.WriteLine($"\n   Специализации: {specializations.Count} записей");
        foreach (var spec in specializations)
        {
            Console.WriteLine($"     - {spec.Name}: {spec.Description}");
        }

        // Проверка записей на прием
        Console.WriteLine($"\n   Записи на прием: {appointments.Count} записей");
        var recentAppointments = appointments
            .OrderByDescending(a => a.AppointmentDateTime);
        foreach (var appointment in recentAppointments)
        {
            Console.WriteLine($"     - {appointment.AppointmentDateTime:dd.MM.yyyy HH:mm}: " +
                            $"{appointment.Patient.FullName} → {appointment.Doctor.FullName} " +
                            $"(каб. {appointment.RoomNumber}, {(appointment.IsFollowUp ? "повторный" : "первичный")})");
        }
    }

    private static void TestRelationships(List<Patient> patients, List<Doctor> doctors, List<Appointment> appointments)
    {
        Console.WriteLine("\n2. ПРОВЕРКА СВЯЗЕЙ МЕЖДУ СУЩНОСТЯМИ:");

        // Проверка, что у всех записей есть пациент и врач
        var appointmentsWithoutPatient = appointments.Where(a => a.Patient == null).Count();
        var appointmentsWithoutDoctor = appointments.Where(a => a.Doctor == null).Count();
        Console.WriteLine($"   Записей без пациента: {appointmentsWithoutPatient}");
        Console.WriteLine($"   Записей без врача: {appointmentsWithoutDoctor}");

        // Проверка связей пациент-врач
        var patientDoctorRelations = appointments
            .GroupBy(a => a.Patient.PassportNumber)
            .Select(g => new { Patient = g.Key, DoctorCount = g.Select(a => a.Doctor.PassportNumber).Distinct().Count() })
            .OrderByDescending(x => x.DoctorCount);

        Console.WriteLine($"\n   Пациенты по количеству врачей:");
        foreach (var relation in patientDoctorRelations)
        {
            var patient = patients.First(p => p.PassportNumber == relation.Patient);
            Console.WriteLine($"     - {patient.FullName}: {relation.DoctorCount} врачей");
        }

        // Проверка загруженности врачей
        var doctorWorkload = appointments
            .GroupBy(a => a.Doctor.PassportNumber)
            .Select(g => new { Doctor = g.Key, AppointmentCount = g.Count() })
            .OrderByDescending(x => x.AppointmentCount);

        Console.WriteLine($"\n   Врачи по количеству приемов:");
        foreach (var workload in doctorWorkload)
        {
            var doctor = doctors.First(d => d.PassportNumber == workload.Doctor);
            Console.WriteLine($"     - {doctor.FullName}: {workload.AppointmentCount} приемов");
        }
    }

    private static void TestBusinessLogic(IPolyclinicService service)
    {
        Console.WriteLine("\n3. ПРОВЕРКА БИЗНЕС-ЛОГИКИ:");

        // Тест 1: Врачи со стажем ≥10 лет
        var experiencedDoctors = service.GetDoctorsWithExperience(10).ToList();
        Console.WriteLine($"   Врачи со стажем ≥10 лет: {experiencedDoctors.Count}");
        foreach (var doctor in experiencedDoctors)
        {
            Console.WriteLine($"     - {doctor.FullName} ({doctor.ExperienceYears} лет)");
        }

        // Тест 2: Пациенты конкретного врача
        if (experiencedDoctors.Any())
        {
            var doctorPassport = experiencedDoctors.First().PassportNumber;
            var doctorPatients = service.GetPatientsByDoctorOrderedByName(doctorPassport).ToList();
            Console.WriteLine($"\n   Пациенты врача {experiencedDoctors.First().FullName}: {doctorPatients.Count}");
            foreach (var patient in doctorPatients)
            {
                Console.WriteLine($"     - {patient.FullName}");
            }
        }

        // Тест 3: Повторные приемы за последний месяц
        var followUpCount = service.GetFollowUpAppointmentsCountLastMonth();
        Console.WriteLine($"\n   Повторные приемы за последний месяц: {followUpCount}");

        // Тест 4: Пациенты старше 30 лет с несколькими врачами
        var patientsOver30MultiDoctors = service.GetPatientsOver30WithMultipleDoctors().ToList();
        Console.WriteLine($"\n   Пациенты >30 лет с несколькими врачами: {patientsOver30MultiDoctors.Count}");
        foreach (var patient in patientsOver30MultiDoctors)
        {
            Console.WriteLine($"     - {patient.FullName} ({patient.Age} лет)");
        }

        // Тест 5: Приемы в кабинете за текущий месяц
        var roomAppointments = service.GetAppointmentsInRoomForCurrentMonth("101").ToList();
        Console.WriteLine($"\n   Приемы в кабинете 101 за текущий месяц: {roomAppointments.Count}");
        foreach (var appointment in roomAppointments)
        {
            Console.WriteLine($"     - {appointment.AppointmentDateTime:dd.MM.yyyy HH:mm}: {appointment.Patient.FullName}");
        }
    }

    private static void TestDataIntegrity(List<Patient> patients, List<Doctor> doctors, List<Appointment> appointments)
    {
        Console.WriteLine("\n4. ПРОВЕРКА ЦЕЛОСТНОСТИ ДАННЫХ:");

        // Проверка уникальности паспортных данных
        var duplicatePatientPassports = patients
            .GroupBy(p => p.PassportNumber)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        Console.WriteLine($"   Дубликаты паспортов пациентов: {duplicatePatientPassports.Count}");

        var duplicateDoctorPassports = doctors
            .GroupBy(d => d.PassportNumber)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        Console.WriteLine($"   Дубликаты паспортов врачей: {duplicateDoctorPassports.Count}");

        // Проверка корректности дат
        var futureAppointments = appointments.Where(a => a.AppointmentDateTime > DateTime.Now.AddYears(1)).Count();
        var pastAppointments = appointments.Where(a => a.AppointmentDateTime < DateTime.Now.AddYears(-5)).Count();
        Console.WriteLine($"   Приемов в далеком будущем (>1 года): {futureAppointments}");
        Console.WriteLine($"   Приемов в далеком прошлом (>5 лет): {pastAppointments}");

        // Проверка возраста
        var invalidAgePatients = patients.Where(p => p.Age < 0 || p.Age > 150).Count();
        var invalidExperienceDoctors = doctors.Where(d => d.ExperienceYears < 0 || d.ExperienceYears > 60).Count();
        Console.WriteLine($"   Пациентов с некорректным возрастом: {invalidAgePatients}");
        Console.WriteLine($"   Врачей с некорректным стажем: {invalidExperienceDoctors}");

        // Проверка формата телефонов
        var invalidPhonePatients = patients.Where(p => string.IsNullOrEmpty(p.PhoneNumber) || !p.PhoneNumber.StartsWith("+")).Count();
        Console.WriteLine($"   Пациентов с некорректным телефоном: {invalidPhonePatients}");
    }

    private static void PrintDatabaseStatistics(List<Patient> patients, List<Doctor> doctors, 
                                              List<Appointment> appointments, List<Specialization> specializations)
    {
        Console.WriteLine("\n5. СТАТИСТИКА БАЗЫ ДАННЫХ:");

        // Общая статистика
        Console.WriteLine($"   Общее количество записей:");
        Console.WriteLine($"     - Пациенты: {patients.Count}");
        Console.WriteLine($"     - Врачи: {doctors.Count}");
        Console.WriteLine($"     - Записи на прием: {appointments.Count}");
        Console.WriteLine($"     - Специализации: {specializations.Count}");

        // Статистика по полу
        var malePatients = patients.Count(p => p.Gender == Gender.Male);
        var femalePatients = patients.Count(p => p.Gender == Gender.Female);
        Console.WriteLine($"\n   Статистика по полу пациентов:");
        Console.WriteLine($"     - Мужчины: {malePatients} ({malePatients * 100.0 / patients.Count:F1}%)");
        Console.WriteLine($"     - Женщины: {femalePatients} ({femalePatients * 100.0 / patients.Count:F1}%)");

        // Статистика по группам крови
        var bloodGroupStats = patients
            .GroupBy(p => p.BloodGroup)
            .Select(g => new { Group = g.Key, Count = g.Count(), Percentage = g.Count() * 100.0 / patients.Count })
            .OrderByDescending(x => x.Count);
        
        Console.WriteLine($"\n   Распределение по группам крови:");
        foreach (var stat in bloodGroupStats)
        {
            Console.WriteLine($"     - {stat.Group}: {stat.Count} ({stat.Percentage:F1}%)");
        }

        // Статистика по специализациям
        var specializationStats = doctors
            .GroupBy(d => d.Specialization.Name)
            .Select(g => new { Specialization = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count);
        
        Console.WriteLine($"\n   Распределение врачей по специализациям:");
        foreach (var stat in specializationStats)
        {
            Console.WriteLine($"     - {stat.Specialization}: {stat.Count} врачей");
        }

        // Статистика по времени
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;
        var appointmentsThisMonth = appointments.Count(a => 
            a.AppointmentDateTime.Month == currentMonth && 
            a.AppointmentDateTime.Year == currentYear);
        var followUpRate = appointments.Count(a => a.IsFollowUp) * 100.0 / appointments.Count;
        
        Console.WriteLine($"\n   Временная статистика:");
        Console.WriteLine($"     - Приемов в текущем месяце: {appointmentsThisMonth}");
        Console.WriteLine($"     - Процент повторных приемов: {followUpRate:F1}%");

        // Статистика по кабинетам
        var roomStats = appointments
            .GroupBy(a => a.RoomNumber)
            .Select(g => new { Room = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count);
        
        Console.WriteLine($"\n   Загруженность кабинетов:");
        foreach (var stat in roomStats)
        {
            Console.WriteLine($"     - Каб. {stat.Room}: {stat.Count} приемов");
        }
    }
}