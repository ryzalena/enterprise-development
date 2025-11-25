using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.Dtos;
using Domain.Entities;
using Domain.Enums;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPolyclinicService _service;

    public PatientsController(IPolyclinicService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<PatientDto>>> GetPatients()
    {
        var patients = await _service.GetPatientsAsync();
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
            PhoneNumber = p.PhoneNumber,
            Age = p.Age
        }).ToList();
        
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDto>> GetPatient(int id)
    {
        var patient = await _service.GetPatientByIdAsync(id);
        if (patient == null) return NotFound();
        
        var dto = new PatientDto
        {
            Id = patient.Id,
            PassportNumber = patient.PassportNumber,
            FullName = patient.FullName,
            Gender = patient.Gender.ToString(),
            BirthDate = patient.BirthDate,
            Address = patient.Address,
            BloodGroup = patient.BloodGroup.ToString(),
            RhFactor = patient.RhFactor.ToString(),
            PhoneNumber = patient.PhoneNumber,
            Age = patient.Age
        };
        
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<PatientDto>> CreatePatient(CreatePatientDto dto)
    {
        var patient = new Patient
        {
            Id = 0, // будет установлен в сервисе
            PassportNumber = dto.PassportNumber,
            FullName = dto.FullName,
            Gender = Enum.Parse<Gender>(dto.Gender),
            BirthDate = dto.BirthDate,
            Address = dto.Address,
            BloodGroup = Enum.Parse<BloodGroup>(dto.BloodGroup),
            RhFactor = Enum.Parse<RhFactor>(dto.RhFactor),
            PhoneNumber = dto.PhoneNumber
        };
        
        var created = await _service.CreatePatientAsync(patient);
        
        var resultDto = new PatientDto
        {
            Id = created.Id,
            PassportNumber = created.PassportNumber,
            FullName = created.FullName,
            Gender = created.Gender.ToString(),
            BirthDate = created.BirthDate,
            Address = created.Address,
            BloodGroup = created.BloodGroup.ToString(),
            RhFactor = created.RhFactor.ToString(),
            PhoneNumber = created.PhoneNumber,
            Age = created.Age
        };
        
        return CreatedAtAction(nameof(GetPatient), new { id = created.Id }, resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePatient(int id, UpdatePatientDto dto)
    {
        var existingPatient = await _service.GetPatientByIdAsync(id);
        if (existingPatient == null) return NotFound();

        existingPatient.PassportNumber = dto.PassportNumber;
        existingPatient.FullName = dto.FullName;
        existingPatient.Gender = Enum.Parse<Gender>(dto.Gender);
        existingPatient.BirthDate = dto.BirthDate;
        existingPatient.Address = dto.Address;
        existingPatient.BloodGroup = Enum.Parse<BloodGroup>(dto.BloodGroup);
        existingPatient.RhFactor = Enum.Parse<RhFactor>(dto.RhFactor);
        existingPatient.PhoneNumber = dto.PhoneNumber;
        
        var updated = await _service.UpdatePatientAsync(id, existingPatient);
        if (updated == null) return NotFound();
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var result = await _service.DeletePatientAsync(id);
        if (!result) return NotFound();
        
        return NoContent();
    }

    [HttpGet("{id}/appointments")]
    public async Task<ActionResult<List<AppointmentDto>>> GetPatientAppointments(int id)
    {
        var appointments = await _service.GetAppointmentsByPatientAsync(id);
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