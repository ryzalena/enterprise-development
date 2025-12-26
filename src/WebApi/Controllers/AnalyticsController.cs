﻿using Microsoft.AspNetCore.Mvc;
using Application.Dtos;
using Domain.Interfaces;

namespace WebApi.Controllers;

/// <summary>
/// Контроллер для аналитики и отчетов
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class AnalyticsController(
    IDoctorService doctorService,
    IPatientService patientService,
    IAppointmentService appointmentService) : ControllerBase
{
    /// <summary>
    /// Получить врачей с опытом работы не менее указанного количества лет
    /// </summary>
    /// <param name="minYears">Минимальное количество лет опыта</param>
    /// <returns>Список врачей с указанным опытом</returns>
    [HttpGet("doctors/experience/{minYears}")]
    public async Task<ActionResult<List<DoctorDto>>> GetDoctorsWithExperience(int minYears)
    {
        var doctors = await doctorService.GetDoctorsWithExperienceAsync(minYears);
        var dtos = doctors.Select(d => new DoctorDto
        {
            Id = d.Id,
            PassportNumber = d.PassportNumber,
            FullName = d.FullName,
            BirthYear = d.BirthYear,
            SpecializationName = d.Specialization?.Name ?? string.Empty,
            ExperienceYears = d.ExperienceYears
        }).ToList();
        
        return Ok(dtos);
    }

    /// <summary>
    /// Получить пациентов по врачу
    /// </summary>
    /// <param name="doctorId">Идентификатор врача</param>
    /// <returns>Список пациентов врача</returns>
    [HttpGet("doctors/{doctorId}/patients")]
    public async Task<ActionResult<List<PatientDto>>> GetPatientsByDoctor(int doctorId)
    {
        var patients = await patientService.GetPatientsByDoctorAsync(doctorId);
        var dtos = patients.Select(p => new PatientDto
        {
            Id = p.Id,
            PassportNumber = p.PassportNumber,
            FullName = p.FullName,
            Gender = p.Gender.ToString(),
            BirthDate = p.BirthDate,
            Address = p.Address,
            BloodGroup = p.BloodGroup.ToString(),
            RhFactor = p.RhFactor.ToString(),
            PhoneNumber = p.PhoneNumber
        }).ToList();
        
        return Ok(dtos);
    }

    /// <summary>
    /// Получить количество повторных приемов за последний месяц
    /// </summary>
    /// <returns>Количество повторных приемов</returns>
    [HttpGet("appointments/follow-up/last-month")]
    public async Task<ActionResult<int>> GetFollowUpAppointmentsCountLastMonth()
    {
        var count = await appointmentService.GetFollowUpCountLastMonthAsync();
        return Ok(new { count });
    }

    /// <summary>
    /// Получить пациентов старше 30 лет, посещающих нескольких врачей
    /// </summary>
    /// <returns>Список пациентов</returns>
    [HttpGet("patients/over-30-multiple-doctors")]
    public async Task<ActionResult<List<PatientDto>>> GetPatientsOver30WithMultipleDoctors()
    {
        var patients = await patientService.GetPatientsOverAgeAsync(30);
        var dtos = patients.Select(p => new PatientDto
        {
            Id = p.Id,
            PassportNumber = p.PassportNumber,
            FullName = p.FullName,
            Gender = p.Gender.ToString(),
            BirthDate = p.BirthDate,
            Address = p.Address,
            BloodGroup = p.BloodGroup.ToString(),
            RhFactor = p.RhFactor.ToString(),
            PhoneNumber = p.PhoneNumber
        }).ToList();
        
        return Ok(dtos);
    }

    /// <summary>
    /// Получить записи на прием в указанном кабинете за текущий месяц
    /// </summary>
    /// <param name="roomNumber">Номер кабинета</param>
    /// <returns>Список записей на прием</returns>
    [HttpGet("appointments/room/{roomNumber}/current-month")]
    public async Task<ActionResult<List<AppointmentDto>>> GetAppointmentsInRoomForCurrentMonth(string roomNumber)
    {
        var appointments = await appointmentService.GetAppointmentsByRoomAndDateAsync(roomNumber, DateTime.Now);
        var dtos = appointments.Select(a => new AppointmentDto
        {
            Id = a.Id,
            PatientId = a.PatientId,
            DoctorId = a.DoctorId,
            AppointmentDateTime = a.AppointmentDateTime,
            RoomNumber = a.RoomNumber,
            IsFollowUp = a.IsFollowUp,
            Patient = a.Patient != null ? new PatientDto
            {
                Id = a.Patient.Id,
                PassportNumber = a.Patient.PassportNumber,
                FullName = a.Patient.FullName,
                Gender = a.Patient.Gender.ToString(),
                BirthDate = a.Patient.BirthDate,
                Address = a.Patient.Address,
                BloodGroup = a.Patient.BloodGroup.ToString(),
                RhFactor = a.Patient.RhFactor.ToString(),
                PhoneNumber = a.Patient.PhoneNumber
            } : null,
            Doctor = a.Doctor != null ? new DoctorDto
            {
                Id = a.Doctor.Id,
                PassportNumber = a.Doctor.PassportNumber,
                FullName = a.Doctor.FullName,
                BirthYear = a.Doctor.BirthYear,
                SpecializationName = a.Doctor.Specialization?.Name ?? string.Empty,
                ExperienceYears = a.Doctor.ExperienceYears
            } : null
        }).ToList();
        
        return Ok(dtos);
    }
}