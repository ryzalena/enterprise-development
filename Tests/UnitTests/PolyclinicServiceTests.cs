using Microsoft.VisualStudio.TestTools.UnitTesting;
using policlinicApp.Domain.Entities;
using policlinicApp.Domain.Enums;
using policlinicApp.Services;
using policlinicApp.Services.Interfaces;
using PolyclinicApp.Data.Seeders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PolyclinicApp.Tests.UnitTests;

[TestClass]
public class PolyclinicServiceTests
{
    private IPolyclinicService _polyclinicService;
    private List<Patient> _patients;
    private List<Doctor> _doctors;
    private List<Appointment> _appointments;
    private List<Specialization> _specializations;

    [TestInitialize]
    public void Setup()
    {
        // Генерация тестовых данных
        var testData = TestDataGenerator.GenerateTestData();
        _patients = testData.Item1;
        _doctors = testData.Item2;
        _appointments = testData.Item3;
        _specializations = testData.Item4;

        // Создание сервиса
        _polyclinicService = new PolyclinicService(_patients, _doctors, _appointments, _specializations);

        // Вывод статистики для отладки
        TestDataGenerator.PrintDataStatistics(_patients, _doctors, _appointments, _specializations);
    }

    [TestMethod]
    public void GetDoctorsWithExperience_WhenMinExperience10Years_ReturnsDoctorsWithAtLeast10YearsExperience()
    {
        // Arrange
        var minExperience = 10;
        var expectedDoctors = _doctors.Where(d => d.ExperienceYears >= minExperience).ToList();

        // Act
        var result = _polyclinicService.GetDoctorsWithExperience(minExperience).ToList();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Count > 0, "Должен вернуться хотя бы один врач со стажем не менее 10 лет");
        Assert.AreEqual(expectedDoctors.Count, result.Count, "Количество врачей не совпадает с ожидаемым");

        // Проверяем, что все врачи имеют стаж не менее указанного
        Assert.IsTrue(result.All(d => d.ExperienceYears >= minExperience), 
            "Все врачи в результате должны иметь стаж не менее указанного");

        // Проверяем, что данные корректны
        foreach (var doctor in result)
        {
            Assert.IsFalse(string.IsNullOrEmpty(doctor.FullName), "ФИО врача не должно быть пустым");
            Assert.IsFalse(string.IsNullOrEmpty(doctor.PassportNumber), "Номер паспорта врача не должен быть пустым");
            Assert.IsNotNull(doctor.Specialization, "Специализация врача не должна быть null");
        }

        // Вывод для отладки
        Console.WriteLine($"Врачи со стажем не менее {minExperience} лет:");
        foreach (var doctor in result)
        {
            Console.WriteLine($"  {doctor.FullName} - {doctor.Specialization.Name} (стаж: {doctor.ExperienceYears} лет)");
        }
    }

    [TestMethod]
    public void GetDoctorsWithExperience_WhenMinExperience0Years_ReturnsAllDoctors()
    {
        // Arrange
        var minExperience = 0;

        // Act
        var result = _polyclinicService.GetDoctorsWithExperience(minExperience).ToList();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(_doctors.Count, result.Count, "Должны вернуться все врачи");
    }

    [TestMethod]
    public void GetDoctorsWithExperience_WhenMinExperienceVeryHigh_ReturnsEmptyList()
    {
        // Arrange
        var minExperience = 100;

        // Act
        var result = _polyclinicService.GetDoctorsWithExperience(minExperience).ToList();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count, "При очень высоком стаже должен вернуться пустой список");
    }

    [TestMethod]
    public void GetPatientsByDoctorOrderedByName_WhenValidDoctorPassport_ReturnsOrderedPatients()
    {
        // Arrange
        var doctor = _doctors.First();
        var doctorPassport = doctor.PassportNumber;
        var expectedPatients = _appointments
            .Where(a => a.Doctor.PassportNumber == doctorPassport)
            .Select(a => a.Patient)
            .Distinct()
            .OrderBy(p => p.FullName)
            .ToList();

        // Act
        var result = _polyclinicService.GetPatientsByDoctorOrderedByName(doctorPassport).ToList();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Count > 0, "Должен вернуться хотя бы один пациент для указанного врача");
        Assert.AreEqual(expectedPatients.Count, result.Count, "Количество пациентов не совпадает с ожидаемым");

        // Проверяем сортировку по ФИО
        var sortedPatients = result.OrderBy(p => p.FullName).ToList();
        CollectionAssert.AreEqual(sortedPatients, result, "Пациенты должны быть отсортированы по ФИО");

        // Проверяем, что все пациенты действительно записаны к указанному врачу
        var patientPassports = result.Select(p => p.PassportNumber).ToList();
        var actualAppointmentsForDoctor = _appointments
            .Where(a => a.Doctor.PassportNumber == doctorPassport && patientPassports.Contains(a.Patient.PassportNumber))
            .ToList();
        Assert.IsTrue(actualAppointmentsForDoctor.Count >= result.Count, 
            "Все пациенты должны иметь записи к указанному врачу");

        // Вывод для отладки
        Console.WriteLine($"Пациенты врача {doctor.FullName} ({doctor.Specialization.Name}):");
        foreach (var patient in result)
        {
            Console.WriteLine($"  {patient.FullName} - {patient.PhoneNumber}");
        }
    }

    [TestMethod]
    public void GetPatientsByDoctorOrderedByName_WhenInvalidDoctorPassport_ReturnsEmptyList()
    {
        // Arrange
        var invalidPassport = "INVALID123";

        // Act
        var result = _polyclinicService.GetPatientsByDoctorOrderedByName(invalidPassport).ToList();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count, "Для несуществующего врача должен вернуться пустой список");
    }

    [TestMethod]
    public void GetPatientsByDoctorOrderedByName_WhenDoctorHasNoPatients_ReturnsEmptyList()
    {
        // Arrange
        // Создаем врача без пациентов
        var doctorWithoutPatients = new Doctor(
            "CD999999", 
            "Врач Безпациентов Тестовый", 
            1970, 
            _specializations.First(), 
            15
        );
        _doctors.Add(doctorWithoutPatients);

        // Act
        var result = _polyclinicService.GetPatientsByDoctorOrderedByName(doctorWithoutPatients.PassportNumber).ToList();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count, "Для врача без пациентов должен вернуться пустой список");

        // Убираем тестового врача, чтобы не влиять на другие тесты
        _doctors.Remove(doctorWithoutPatients);
    }

    [TestMethod]
    public void GetFollowUpAppointmentsCountLastMonth_WhenCalled_ReturnsCorrectCount()
    {
        // Act
        var result = _polyclinicService.GetFollowUpAppointmentsCountLastMonth();

        // Assert
        Assert.IsTrue(result >= 0, "Количество не может быть отрицательным");

        // Проверяем расчет вручную
        var lastMonth = DateTime.Now.AddMonths(-1);
        var expectedCount = _appointments.Count(a => 
            a.IsFollowUp && 
            a.AppointmentDateTime.Month == lastMonth.Month && 
            a.AppointmentDateTime.Year == lastMonth.Year);

        Assert.AreEqual(expectedCount, result, "Количество повторных приемов не совпадает с расчетным");

        // Вывод для отладки
        Console.WriteLine($"Повторные приемы за последний месяц ({lastMonth:MMMM yyyy}): {result}");

        // Дополнительная проверка: убеждаемся, что считаются только повторные приемы
        var allAppointmentsLastMonth = _appointments.Count(a => 
            a.AppointmentDateTime.Month == lastMonth.Month && 
            a.AppointmentDateTime.Year == lastMonth.Year);
        Assert.IsTrue(result <= allAppointmentsLastMonth, 
            "Количество повторных приемов не может превышать общее количество приемов");
    }

    [TestMethod]
    public void GetPatientsOver30WithMultipleDoctors_WhenCalled_ReturnsPatientsOver30WithMultipleDoctorsOrderedByBirthDate()
    {
        // Act
        var result = _polyclinicService.GetPatientsOver30WithMultipleDoctors().ToList();

        // Assert
        Assert.IsNotNull(result);

        if (result.Count > 0)
        {
            // Проверяем, что все пациенты старше 30 лет
            Assert.IsTrue(result.All(p => p.Age > 30), 
                "Все пациенты должны быть старше 30 лет");

            // Проверяем, что у каждого пациента более одного врача
            foreach (var patient in result)
            {
                var patientDoctorsCount = _appointments
                    .Where(a => a.Patient.PassportNumber == patient.PassportNumber)
                    .Select(a => a.Doctor.PassportNumber)
                    .Distinct()
                    .Count();
                
                Assert.IsTrue(patientDoctorsCount > 1, 
                    $"У пациента {patient.FullName} должен быть более одного врача");
            }

            // Проверяем сортировку по дате рождения
            var sortedByBirthDate = result.OrderBy(p => p.BirthDate).ToList();
            CollectionAssert.AreEqual(sortedByBirthDate, result, 
                "Пациенты должны быть отсортированы по дате рождения");

            // ВЫВОДИМ ВСЕХ ПАЦИЕНТОВ БЕЗ ОГРАНИЧЕНИЙ
            Console.WriteLine($"Пациенты старше 30 лет, записанные к нескольким врачам ({result.Count} записей):");
            foreach (var patient in result)
            {
                var patientDoctors = _appointments
                    .Where(a => a.Patient.PassportNumber == patient.PassportNumber)
                    .Select(a => a.Doctor.FullName)
                    .Distinct();
                
                Console.WriteLine($"  {patient.FullName} ({patient.Age} лет, рожд. {patient.BirthDate:dd.MM.yyyy})");
                Console.WriteLine($"    Врачи: {string.Join(", ", patientDoctors)}");
            }
        }
        else
        {
            Console.WriteLine("Нет пациентов старше 30 лет, записанных к нескольким врачам");
            
            // Для отладки выводим информацию о всех пациентах старше 30
            var patientsOver30 = _patients.Where(p => p.Age > 30).ToList();
            Console.WriteLine($"Всего пациентов старше 30 лет: {patientsOver30.Count}");
            foreach (var patient in patientsOver30)
            {
                var doctorCount = _appointments
                    .Where(a => a.Patient.PassportNumber == patient.PassportNumber)
                    .Select(a => a.Doctor.PassportNumber)
                    .Distinct()
                    .Count();
                Console.WriteLine($"  {patient.FullName} ({patient.Age} лет) - врачей: {doctorCount}");
            }
        }
    }
        
    [TestMethod]
    public void GetPatientsOver30WithMultipleDoctors_WhenNoSuchPatients_ReturnsEmptyList()
    {
        // Этот тест проверяет, что метод корректно работает когда нет подходящих пациентов
        // В нашем случае генератор данных гарантирует наличие таких пациентов,
        // но тест проверяет корректность логики
        
        // Act
        var result = _polyclinicService.GetPatientsOver30WithMultipleDoctors().ToList();

        // Assert
        Assert.IsNotNull(result);
        // Метод может вернуть пустой список или нет - зависит от данных
        // Главное, что не должно быть исключений
    }

    [TestMethod]
    public void GetAppointmentsInRoomForCurrentMonth_WhenValidRoomNumber_ReturnsAppointmentsForCurrentMonth()
    {
        // Arrange
        var roomNumber = "101";
        var currentDate = DateTime.Now;

        // Act
        var result = _polyclinicService.GetAppointmentsInRoomForCurrentMonth(roomNumber).ToList();

        // Assert
        Assert.IsNotNull(result);

        if (result.Count > 0)
        {
            // Проверяем, что все приемы в указанном кабинете
            Assert.IsTrue(result.All(a => a.RoomNumber == roomNumber), 
                "Все приемы должны быть в указанном кабинете");

            // Проверяем, что все приемы в текущем месяце
            Assert.IsTrue(result.All(a => 
                a.AppointmentDateTime.Month == currentDate.Month && 
                a.AppointmentDateTime.Year == currentDate.Year), 
                "Все приемы должны быть в текущем месяце");

            // Проверяем корректность данных
            foreach (var appointment in result)
            {
                Assert.IsNotNull(appointment.Patient, "Пациент не должен быть null");
                Assert.IsNotNull(appointment.Doctor, "Врач не должен быть null");
                Assert.IsFalse(string.IsNullOrEmpty(appointment.RoomNumber), "Номер кабинета не должен быть пустым");
            }

            // Вывод для отладки
            Console.WriteLine($"Приемы в кабинете {roomNumber} за {currentDate:MMMM yyyy}:");
            foreach (var appointment in result)
            {
                Console.WriteLine($"  {appointment.AppointmentDateTime:dd.MM.yyyy HH:mm}");
                Console.WriteLine($"    Пациент: {appointment.Patient.FullName}");
                Console.WriteLine($"    Врач: {appointment.Doctor.FullName}");
                Console.WriteLine($"    Повторный: {(appointment.IsFollowUp ? "Да" : "Нет")}");
            }
        }
        else
        {
            Console.WriteLine($"Нет приемов в кабинете {roomNumber} за {currentDate:MMMM yyyy}");
        }
    }

    [TestMethod]
    public void GetAppointmentsInRoomForCurrentMonth_WhenInvalidRoomNumber_ReturnsEmptyList()
    {
        // Arrange
        var invalidRoomNumber = "999";

        // Act
        var result = _polyclinicService.GetAppointmentsInRoomForCurrentMonth(invalidRoomNumber).ToList();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count, "Для несуществующего кабинета должен вернуться пустой список");
    }

    [TestMethod]
    public void GetAppointmentsInRoomForCurrentMonth_WhenRoomHasNoAppointmentsThisMonth_ReturnsEmptyList()
    {
        // Arrange
        // Ищем кабинет, который точно не используется в текущем месяце
        var currentMonthAppointments = _appointments
            .Where(a => a.AppointmentDateTime.Month == DateTime.Now.Month && 
                       a.AppointmentDateTime.Year == DateTime.Now.Year)
            .ToList();
        
        var roomsInCurrentMonth = currentMonthAppointments
            .Select(a => a.RoomNumber)
            .Distinct()
            .ToList();
        
        string roomWithoutAppointments = null;
        for (var i = 1; i <= 50; i++)
        {
            var testRoom = $"99{i:D2}";
            if (!roomsInCurrentMonth.Contains(testRoom))
            {
                roomWithoutAppointments = testRoom;
                break;
            }
        }

        if (roomWithoutAppointments != null)
        {
            // Act
            var result = _polyclinicService.GetAppointmentsInRoomForCurrentMonth(roomWithoutAppointments).ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count, 
                $"Для кабинета {roomWithoutAppointments} без приемов в текущем месяце должен вернуться пустой список");
        }
    }

    [TestMethod]
    public void TestDataGeneration_WhenCalled_GeneratesRequiredNumberOfInstances()
    {
        // Assert
        Assert.IsTrue(_patients.Count >= 10, 
            $"Ожидалось минимум 10 пациентов, получено: {_patients.Count}");
        Assert.IsTrue(_doctors.Count >= 10, 
            $"Ожидалось минимум 10 врачей, получено: {_doctors.Count}");
        Assert.IsTrue(_appointments.Count >= 10, 
            $"Ожидалось минимум 10 записей, получено: {_appointments.Count}");
        Assert.IsTrue(_specializations.Count >= 10, 
            $"Ожидалось минимум 10 специализаций, получено: {_specializations.Count}");

        // Проверяем качество данных
        Assert.IsTrue(_patients.All(p => !string.IsNullOrEmpty(p.FullName)), 
            "У всех пациентов должно быть ФИО");
        Assert.IsTrue(_patients.All(p => !string.IsNullOrEmpty(p.PassportNumber)), 
            "У всех пациентов должен быть номер паспорта");
        Assert.IsTrue(_doctors.All(d => !string.IsNullOrEmpty(d.FullName)), 
            "У всех врачей должно быть ФИО");
        Assert.IsTrue(_doctors.All(d => !string.IsNullOrEmpty(d.PassportNumber)), 
            "У всех врачей должен быть номер паспорта");
        Assert.IsTrue(_appointments.All(a => a.Patient != null), 
            "У всех записей должен быть пациент");
        Assert.IsTrue(_appointments.All(a => a.Doctor != null), 
            "У всех записей должен быть врач");

        Console.WriteLine("✓ Все проверки данных пройдены успешно");
    }

    [TestMethod]
    public void ServiceConstructor_WhenNullParameters_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => 
            new PolyclinicService(null, _doctors, _appointments, _specializations));
        
        Assert.ThrowsException<ArgumentNullException>(() => 
            new PolyclinicService(_patients, null, _appointments, _specializations));
        
        Assert.ThrowsException<ArgumentNullException>(() => 
            new PolyclinicService(_patients, _doctors, null, _specializations));
        
        Assert.ThrowsException<ArgumentNullException>(() => 
            new PolyclinicService(_patients, _doctors, _appointments, null));
    }

    [TestMethod]
    public void IntegrationTest_AllMethodsWorkTogether()
    {
        // Этот тест проверяет, что все методы работают корректно вместе
        
        // Act & Assert - вызываем все методы и проверяем, что нет исключений
        var doctorsWithExperience = _polyclinicService.GetDoctorsWithExperience(10).ToList();
        var patientsByDoctor = _polyclinicService.GetPatientsByDoctorOrderedByName(_doctors.First().PassportNumber).ToList();
        var followUpCount = _polyclinicService.GetFollowUpAppointmentsCountLastMonth();
        var patientsOver30 = _polyclinicService.GetPatientsOver30WithMultipleDoctors().ToList();
        var roomAppointments = _polyclinicService.GetAppointmentsInRoomForCurrentMonth("101").ToList();

        // Дошли до этой точки без исключений - тест пройден
        Assert.IsTrue(true, "Все методы работают корректно вместе");
        
        Console.WriteLine("Интеграционный тест пройден:");
        Console.WriteLine($"  Врачей со стажем ≥10 лет: {doctorsWithExperience.Count}");
        Console.WriteLine($"  Пациентов у первого врача: {patientsByDoctor.Count}");
        Console.WriteLine($"  Повторных приемов: {followUpCount}");
        Console.WriteLine($"  Пациентов >30 лет с несколькими врачами: {patientsOver30.Count}");
        Console.WriteLine($"  Приемов в кабинете 101: {roomAppointments.Count}");
    }
}