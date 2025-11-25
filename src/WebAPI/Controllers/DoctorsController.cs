using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.Dtos;
using Domain.Entities;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IPolyclinicService _service;

    public DoctorsController(IPolyclinicService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<DoctorDto>>> GetDoctors()
    {
        var doctors = await _service.GetDoctorsAsync();
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

    [HttpGet("{id}")]
    public async Task<ActionResult<DoctorDto>> GetDoctor(int id)
    {
        var doctor = await _service.GetDoctorByIdAsync(id);
        if (doctor == null) return NotFound();
        
        var dto = new DoctorDto
        {
            Id = doctor.Id,
            PassportNumber = doctor.PassportNumber,
            FullName = doctor.FullName,
            BirthYear = doctor.BirthYear,
            SpecializationName = doctor.Specialization.Name,
            ExperienceYears = doctor.ExperienceYears
        };
        
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<DoctorDto>> CreateDoctor(CreateDoctorDto dto)
    {
        var doctor = new Doctor
        {
            Id = 0, // будет установлен в сервисе
            PassportNumber = dto.PassportNumber,
            FullName = dto.FullName,
            BirthYear = dto.BirthYear,
            ExperienceYears = dto.ExperienceYears,
            Specialization = new Specialization 
            { 
                Id = 0, // будет установлен в сервисе
                Name = dto.SpecializationName 
            }
        };
        
        var created = await _service.CreateDoctorAsync(doctor);
        
        var resultDto = new DoctorDto
        {
            Id = created.Id,
            PassportNumber = created.PassportNumber,
            FullName = created.FullName,
            BirthYear = created.BirthYear,
            SpecializationName = created.Specialization.Name,
            ExperienceYears = created.ExperienceYears
        };
        
        return CreatedAtAction(nameof(GetDoctor), new { id = created.Id }, resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDoctor(int id, UpdateDoctorDto dto)
    {
        var existingDoctor = await _service.GetDoctorByIdAsync(id);
        if (existingDoctor == null) return NotFound();

        existingDoctor.PassportNumber = dto.PassportNumber;
        existingDoctor.FullName = dto.FullName;
        existingDoctor.BirthYear = dto.BirthYear;
        existingDoctor.ExperienceYears = dto.ExperienceYears;
        existingDoctor.Specialization.Name = dto.SpecializationName;
        
        var updated = await _service.UpdateDoctorAsync(id, existingDoctor);
        if (updated == null) return NotFound();
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDoctor(int id)
    {
        var result = await _service.DeleteDoctorAsync(id);
        if (!result) return NotFound();
        
        return NoContent();
    }

    [HttpGet("{id}/appointments")]
    public async Task<ActionResult<List<AppointmentDto>>> GetDoctorAppointments(int id)
    {
        var appointments = await _service.GetAppointmentsByDoctorAsync(id);
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