using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.Dtos;
using Domain.Interfaces;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class AnalyticsController(
    IDoctorService doctorService,
    IPatientService patientService,
    IAppointmentService appointmentService) : ControllerBase
{
    private readonly IDoctorService _doctorService = doctorService;
    private readonly IPatientService _patientService = patientService;
    private readonly IAppointmentService _appointmentService = appointmentService;

    [HttpGet("doctors/experience/{minYears}")]
    public async Task<ActionResult<List<DoctorDto>>> GetDoctorsWithExperience(int minYears)
    {
        var doctors = await _doctorService.GetDoctorsWithExperienceAsync(minYears);
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

    [HttpGet("doctors/{doctorId}/patients")]
    public async Task<ActionResult<List<PatientDto>>> GetPatientsByDoctor(int doctorId)
    {
        var patients = await _patientService.GetPatientsByDoctorAsync(doctorId);
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

    [HttpGet("appointments/follow-up/last-month")]
    public async Task<ActionResult<int>> GetFollowUpAppointmentsCountLastMonth()
    {
        var count = await _appointmentService.GetFollowUpCountLastMonthAsync();
        return Ok(new { count });
    }

    [HttpGet("patients/over-30-multiple-doctors")]
    public async Task<ActionResult<List<PatientDto>>> GetPatientsOver30WithMultipleDoctors()
    {
        var patients = await _patientService.GetPatientsOverAgeAsync(30);
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

    [HttpGet("appointments/room/{roomNumber}/current-month")]
    public async Task<ActionResult<List<AppointmentDto>>> GetAppointmentsInRoomForCurrentMonth(string roomNumber)
    {
        var appointments = await _appointmentService.GetAppointmentsByRoomAndDateAsync(roomNumber, DateTime.Now);
        var dtos = appointments.Select(a => new AppointmentDto
        {
            Id = a.Id,
            PatientId = a.PatientId,
            DoctorId = a.DoctorId,
            AppointmentDateTime = a.AppointmentDateTime,
            RoomNumber = a.RoomNumber,
            IsFollowUp = a.IsFollowUp,
            PatientName = a.Patient?.FullName ?? string.Empty,
            DoctorName = a.Doctor?.FullName ?? string.Empty
        }).ToList();
        
        return Ok(dtos);
    }
}