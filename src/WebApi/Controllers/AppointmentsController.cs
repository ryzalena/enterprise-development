using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces;
using Application.Dtos;
using Domain.Entities;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class AppointmentsController(
    IAppointmentService appointmentService,
    IPatientService patientService,
    IDoctorService doctorService) : ControllerBase
{
    private readonly IAppointmentService _appointmentService = appointmentService;
    private readonly IPatientService _patientService = patientService;
    private readonly IDoctorService _doctorService = doctorService;

    [HttpGet]
    public async Task<ActionResult<List<AppointmentDto>>> GetAppointments()
    {
        var appointments = await _appointmentService.GetAllAppointmentsAsync();
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

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDto>> GetAppointment(int id)
    {
        var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
        if (appointment == null) return NotFound();

        var dto = new AppointmentDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            AppointmentDateTime = appointment.AppointmentDateTime,
            RoomNumber = appointment.RoomNumber,
            IsFollowUp = appointment.IsFollowUp,
            PatientName = appointment.Patient?.FullName ?? string.Empty,
            DoctorName = appointment.Doctor?.FullName ?? string.Empty
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> CreateAppointment(
        [FromBody] AppointmentManipulationDto dto)
    {
        var patient = await _patientService.GetPatientByIdAsync(dto.PatientId);
        var doctor = await _doctorService.GetDoctorByIdAsync(dto.DoctorId);

        if (patient == null || doctor == null)
        {
            return BadRequest("Patient or Doctor not found");
        }

        var appointment = new Appointment
        {
            PatientId = dto.PatientId,
            Patient = patient,
            DoctorId = dto.DoctorId,
            Doctor = doctor,
            AppointmentDateTime = dto.AppointmentDateTime,
            RoomNumber = dto.RoomNumber,
            IsFollowUp = dto.IsFollowUp
        };

        var createdAppointment = await _appointmentService.CreateAppointmentAsync(appointment);

        var resultDto = new AppointmentDto
        {
            Id = createdAppointment.Id,
            PatientId = createdAppointment.PatientId,
            DoctorId = createdAppointment.DoctorId,
            AppointmentDateTime = createdAppointment.AppointmentDateTime,
            RoomNumber = createdAppointment.RoomNumber,
            IsFollowUp = createdAppointment.IsFollowUp,
            PatientName = patient.FullName,
            DoctorName = doctor.FullName
        };

        return CreatedAtAction(
            nameof(GetAppointment),
            new { id = createdAppointment.Id },
            resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAppointment(
        int id,
        [FromBody] AppointmentManipulationDto dto)
    {
        var existingAppointment = await _appointmentService.GetAppointmentByIdAsync(id);
        if (existingAppointment == null)
        {
            return NotFound($"Appointment with id {id} not found");
        }

        var patient = await _patientService.GetPatientByIdAsync(dto.PatientId);
        var doctor = await _doctorService.GetDoctorByIdAsync(dto.DoctorId);

        if (patient == null || doctor == null)
        {
            return BadRequest("Patient or Doctor not found");
        }

        existingAppointment.PatientId = dto.PatientId;
        existingAppointment.Patient = patient;
        existingAppointment.DoctorId = dto.DoctorId;
        existingAppointment.Doctor = doctor;
        existingAppointment.AppointmentDateTime = dto.AppointmentDateTime;
        existingAppointment.RoomNumber = dto.RoomNumber;
        existingAppointment.IsFollowUp = dto.IsFollowUp;

        await _appointmentService.UpdateAppointmentAsync(id, existingAppointment);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var existingAppointment = await _appointmentService.GetAppointmentByIdAsync(id);
        if (existingAppointment == null)
        {
            return NoContent();
        }

        await _appointmentService.DeleteAppointmentAsync(id);

        return NoContent();
    }
}