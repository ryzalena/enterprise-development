using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.Dtos;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IPolyclinicService _service;

    public AnalyticsController(IPolyclinicService service)
    {
        _service = service;
    }

    [HttpGet("doctors/experience/{minYears}")]
    public async Task<ActionResult<List<DoctorDto>>> GetDoctorsWithExperience(int minYears)
    {
        var doctors = await _service.GetDoctorsWithExperienceAsync(minYears);
        var dtos = doctors.Select(d => new DoctorDto
        {
            Id = d.Id,
            PassportNumber = d.PassportNumber,
            FullName = d.FullName,
            BirthYear = d.BirthYear,
            SpecializationName = d.Specialization.Name,
            ExperienceYears = d.ExperienceYears
        }).ToList();
        
        return Ok(dtos);
    }

    [HttpGet("doctors/{doctorId}/patients")]
    public async Task<ActionResult<List<PatientDto>>> GetPatientsByDoctor(int doctorId)
    {
        var patients = await _service.GetPatientsByDoctorAsync(doctorId);
        var dtos = patients.Select(p => new PatientDto
        {
            Id = p.Id,
            FullName = p.FullName,
            BirthDate = p.BirthDate,
            PhoneNumber = p.PhoneNumber,
            Age = p.Age
        }).ToList();
        
        return Ok(dtos);
    }

    [HttpGet("appointments/follow-up/last-month")]
    public async Task<ActionResult<int>> GetFollowUpAppointmentsCountLastMonth()
    {
        var count = await _service.GetFollowUpAppointmentsCountLastMonthAsync();
        return Ok(new { count });
    }

    [HttpGet("patients/over-30-multiple-doctors")]
    public async Task<ActionResult<List<PatientDto>>> GetPatientsOver30WithMultipleDoctors()
    {
        var patients = await _service.GetPatientsOver30WithMultipleDoctorsAsync();
        var dtos = patients.Select(p => new PatientDto
        {
            Id = p.Id,
            FullName = p.FullName,
            BirthDate = p.BirthDate,
            PhoneNumber = p.PhoneNumber,
            Age = p.Age
        }).ToList();
        
        return Ok(dtos);
    }

    [HttpGet("appointments/room/{roomNumber}/current-month")]
    public async Task<ActionResult<List<AppointmentDto>>> GetAppointmentsInRoomForCurrentMonth(string roomNumber)
    {
        var appointments = await _service.GetAppointmentsInRoomForCurrentMonthAsync(roomNumber);
        var dtos = appointments.Select(a => new AppointmentDto
        {
            Id = a.Id,
            PatientId = a.PatientId,
            DoctorId = a.DoctorId,
            AppointmentDateTime = a.AppointmentDateTime,
            RoomNumber = a.RoomNumber,
            IsFollowUp = a.IsFollowUp,
            PatientName = a.Patient.FullName,
            DoctorName = a.Doctor.FullName
        }).ToList();
        
        return Ok(dtos);
    }
}