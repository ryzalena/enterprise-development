using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.Dtos;
using Domain.Entities;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IPolyclinicService _service;

    public AppointmentsController(IPolyclinicService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<AppointmentDto>>> GetAppointments()
    {
        var appointments = await _service.GetAppointmentsAsync();
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

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDto>> GetAppointment(int id)
    {
        var appointment = await _service.GetAppointmentByIdAsync(id);
        if (appointment == null) return NotFound();
        
        var dto = new AppointmentDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            AppointmentDateTime = appointment.AppointmentDateTime,
            RoomNumber = appointment.RoomNumber,
            IsFollowUp = appointment.IsFollowUp,
            PatientName = appointment.Patient.FullName,
            DoctorName = appointment.Doctor.FullName
        };
        
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> CreateAppointment(CreateAppointmentDto dto)
    {
        // Получаем пациента и врача для проверки существования
        var patient = await _service.GetPatientByIdAsync(dto.PatientId);
        var doctor = await _service.GetDoctorByIdAsync(dto.DoctorId);
        
        if (patient == null || doctor == null)
        {
            return BadRequest("Patient or Doctor not found");
        }

        var appointment = new Appointment
        {
            Id = 0, // будет установлен в сервисе
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            AppointmentDateTime = dto.AppointmentDateTime,
            RoomNumber = dto.RoomNumber,
            IsFollowUp = dto.IsFollowUp,
            Patient = patient,
            Doctor = doctor
        };
        
        var created = await _service.CreateAppointmentAsync(appointment);
        
        var resultDto = new AppointmentDto
        {
            Id = created.Id,
            PatientId = created.PatientId,
            DoctorId = created.DoctorId,
            AppointmentDateTime = created.AppointmentDateTime,
            RoomNumber = created.RoomNumber,
            IsFollowUp = created.IsFollowUp,
            PatientName = created.Patient.FullName,
            DoctorName = created.Doctor.FullName
        };
        
        return CreatedAtAction(nameof(GetAppointment), new { id = created.Id }, resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAppointment(int id, UpdateAppointmentDto dto)
    {
        var existingAppointment = await _service.GetAppointmentByIdAsync(id);
        if (existingAppointment == null) return NotFound();

        // Получаем пациента и врача для проверки существования
        var patient = await _service.GetPatientByIdAsync(dto.PatientId);
        var doctor = await _service.GetDoctorByIdAsync(dto.DoctorId);
        
        if (patient == null || doctor == null)
        {
            return BadRequest("Patient or Doctor not found");
        }

        existingAppointment.PatientId = dto.PatientId;
        existingAppointment.DoctorId = dto.DoctorId;
        existingAppointment.AppointmentDateTime = dto.AppointmentDateTime;
        existingAppointment.RoomNumber = dto.RoomNumber;
        existingAppointment.IsFollowUp = dto.IsFollowUp;
        existingAppointment.Patient = patient;
        existingAppointment.Doctor = doctor;
        
        var updated = await _service.UpdateAppointmentAsync(id, existingAppointment);
        if (updated == null) return NotFound();
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var result = await _service.DeleteAppointmentAsync(id);
        if (!result) return NotFound();
        
        return NoContent();
    }
}