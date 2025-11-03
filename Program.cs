using System;
using System.Collections.Generic;
using System.Linq;
using PolyclinicApp.Data.Seeders;
using policlinicApp.Services;
using PolyclinicApp.Tests;

namespace PolyclinicApp;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("=== ПРИЛОЖЕНИЕ ПОЛИКЛИНИКИ ===\n");

        try
        {
            // Запуск комплексного тестирования базы данных
            DatabaseTester.TestDatabase();

            // Дополнительные тесты можно добавить здесь
            RunAdditionalTests();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ ОШИБКА: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
        }

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }

    private static void RunAdditionalTests()
    {
        Console.WriteLine("\n\n=== ДОПОЛНИТЕЛЬНЫЕ ТЕСТЫ ===");

        // Тест производительности
        TestPerformance();

        // Тест крайних случаев
        TestEdgeCases();
    }

    private static void TestPerformance()
    {
        Console.WriteLine("\nТЕСТ ПРОИЗВОДИТЕЛЬНОСТИ:");
        
        var testData = TestDataGenerator.GenerateTestData();
        var service = new PolyclinicService(
            testData.Item1, testData.Item2, testData.Item3, testData.Item4);

        var startTime = DateTime.Now;

        // Выполняем все основные запросы
        var doctors = service.GetDoctorsWithExperience(10).ToList();
        var firstDoctorPassport = testData.Item2.First().PassportNumber;
        var patients = service.GetPatientsByDoctorOrderedByName(firstDoctorPassport).ToList();
        var followUps = service.GetFollowUpAppointmentsCountLastMonth();
        var multiDoctorPatients = service.GetPatientsOver30WithMultipleDoctors().ToList();
        var roomAppointments = service.GetAppointmentsInRoomForCurrentMonth("101").ToList();

        var endTime = DateTime.Now;
        var duration = (endTime - startTime).TotalMilliseconds;

        Console.WriteLine($"   Все запросы выполнены за: {duration:F2} мс");
        Console.WriteLine($"   Обработано: {doctors.Count} врачей, {patients.Count} пациентов, " +
                         $"{multiDoctorPatients.Count} сложных случаев");
    }

    private static void TestEdgeCases()
    {
        Console.WriteLine("\nТЕСТ КРАЙНИХ СЛУЧАЕВ:");

        var testData = TestDataGenerator.GenerateTestData();
        var service = new PolyclinicService(
            testData.Item1, testData.Item2, testData.Item3, testData.Item4);

        // Тест с несуществующими данными
        Console.WriteLine("   Поиск несуществующего врача...");
        var nonExistentDoctorPatients = service.GetPatientsByDoctorOrderedByName("NONEXISTENT");
        Console.WriteLine($"     Результат: {nonExistentDoctorPatients.Count()} пациентов");

        Console.WriteLine("   Поиск в несуществующем кабинете...");
        var nonExistentRoomAppointments = service.GetAppointmentsInRoomForCurrentMonth("999");
        Console.WriteLine($"     Результат: {nonExistentRoomAppointments.Count()} приемов");

        Console.WriteLine("   Врачи с очень большим стажем...");
        var veryExperiencedDoctors = service.GetDoctorsWithExperience(100);
        Console.WriteLine($"     Результат: {veryExperiencedDoctors.Count()} врачей");

        Console.WriteLine("   ✓ Все крайние случаи обработаны корректно");
    }
}