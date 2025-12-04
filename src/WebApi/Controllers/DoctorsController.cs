using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces;
using Application.Dtos;
using Domain.Entities;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class DoctorsController(
    IDoctorService doctorService,
    IAppointmentService appointmentService,
    ISpecializationService specializationService) : ControllerBase
{
    private readonly IDoctorService _doctorService = doctorService;
    private readonly IAppointmentService _appointmentService = appointmentService;
    private readonly ISpecializationService _specializationService = specializationService;

    [HttpGet]
    public async Task<ActionResult<List<DoctorDto>>> GetDoctors()
    {
        var doctors = await _doctorService.GetAllDoctorsAsync();
        var dtos = doctors.Select(d => new DoctorDto
        {
            Id = d.Id,
            PassportNumber = d.PassportNumber,
            FullName = d.FullName,
            BirthYear = d.BirthYear,
            SpecializationId = d.SpecializationId,
            SpecializationName = d.Specialization?.Name ?? string.Empty,
            ExperienceYears = d.ExperienceYears
        }).ToList();
        
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DoctorDto>> GetDoctor(int id)
    {
        var doctor = await _doctorService.GetDoctorByIdAsync(id);
        if (doctor == null) return NotFound();
        
        var dto = new DoctorDto
        {
            Id = doctor.Id,
            PassportNumber = doctor.PassportNumber,
            FullName = doctor.FullName,
            BirthYear = doctor.BirthYear,
            SpecializationId = doctor.SpecializationId,
            SpecializationName = doctor.Specialization?.Name ?? string.Empty,
            ExperienceYears = doctor.ExperienceYears
        };
        
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<DoctorDto>> CreateDoctor(
        [FromBody] DoctorManipulationDto dto)
    {
        var specialization = await _specializationService.GetSpecializationByIdAsync(dto.SpecializationId);
        if (specialization == null)
        {
            return BadRequest($"Specialization with id {dto.SpecializationId} not found");
        }

        var doctor = new Doctor
        {
            PassportNumber = dto.PassportNumber,
            FullName = dto.FullName,
            BirthYear = dto.BirthYear,
            ExperienceYears = dto.ExperienceYears,
            SpecializationId = dto.SpecializationId,
            Specialization = specialization
        };
        
        var created = await _doctorService.CreateDoctorAsync(doctor);
        
        var resultDto = new DoctorDto
        {
            Id = created.Id,
            PassportNumber = created.PassportNumber,
            FullName = created.FullName,
            BirthYear = created.BirthYear,
            SpecializationId = created.SpecializationId,
            SpecializationName = specialization.Name,
            ExperienceYears = created.ExperienceYears
        };
        
        return CreatedAtAction(nameof(GetDoctor), new { id = created.Id }, resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDoctor(
        int id, 
        [FromBody] DoctorManipulationDto dto)
    {
        var existingDoctor = await _doctorService.GetDoctorByIdAsync(id);
        if (existingDoctor == null) 
        {
            return NotFound($"Doctor with id {id} not found");
        }

        var specialization = await _specializationService.GetSpecializationByIdAsync(dto.SpecializationId);
        if (specialization == null)
        {
            return BadRequest($"Specialization with id {dto.SpecializationId} not found");
        }

        existingDoctor.PassportNumber = dto.PassportNumber;
        existingDoctor.FullName = dto.FullName;
        existingDoctor.BirthYear = dto.BirthYear;
        existingDoctor.ExperienceYears = dto.ExperienceYears;
        existingDoctor.SpecializationId = dto.SpecializationId;
        existingDoctor.Specialization = specialization;
        
        await _doctorService.UpdateDoctorAsync(id, existingDoctor);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDoctor(int id)
    {
        var existingDoctor = await _doctorService.GetDoctorByIdAsync(id);
        if (existingDoctor == null) 
        {
            return NoContent();
        }
        
        await _doctorService.DeleteDoctorAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/appointments")]
    public async Task<ActionResult<List<AppointmentDto>>> GetDoctorAppointments(int id)
    {
        var doctor = await _doctorService.GetDoctorByIdAsync(id);
        if (doctor == null)
        {
            return NotFound($"Doctor with id {id} not found");
        }

        var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(id);
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