using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces;
using Application.Dtos;
using Domain.Entities;
using Domain.Enums;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly IAppointmentService _appointmentService;

    public PatientsController(
        IPatientService patientService, 
        IAppointmentService appointmentService)
    {
        _patientService = patientService;
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<PatientDto>>> GetPatients()
    {
        var patients = await _patientService.GetAllPatientsAsync();
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

    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDto>> GetPatient(int id)
    {
        var patient = await _patientService.GetPatientByIdAsync(id);
        if (patient == null) 
        {
            return NotFound($"Patient with id {id} not found");
        }
        
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
            PhoneNumber = patient.PhoneNumber
        };
        
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<PatientDto>> CreatePatient(
        [FromBody] PatientManipulationDto dto)
    {
        try
        {
            var patient = new Patient
            {
                Id = 0, // Будет сгенерировано сервисом
                PassportNumber = dto.PassportNumber,
                FullName = dto.FullName,
                Gender = Enum.Parse<Gender>(dto.Gender),
                BirthDate = dto.BirthDate,
                Address = dto.Address,
                BloodGroup = Enum.Parse<BloodGroup>(dto.BloodGroup),
                RhFactor = Enum.Parse<RhFactor>(dto.RhFactor),
                PhoneNumber = dto.PhoneNumber
            };
            
            var created = await _patientService.CreatePatientAsync(patient);
            
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
                PhoneNumber = created.PhoneNumber
            };
            
            return CreatedAtAction(nameof(GetPatient), new { id = created.Id }, resultDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Invalid enum value: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePatient(
        int id, 
        [FromBody] PatientManipulationDto dto)
    {
        var existingPatient = await _patientService.GetPatientByIdAsync(id);
        if (existingPatient == null) 
        {
            return NotFound($"Patient with id {id} not found");
        }

        try
        {
            existingPatient.PassportNumber = dto.PassportNumber;
            existingPatient.FullName = dto.FullName;
            existingPatient.Gender = Enum.Parse<Gender>(dto.Gender);
            existingPatient.BirthDate = dto.BirthDate;
            existingPatient.Address = dto.Address;
            existingPatient.BloodGroup = Enum.Parse<BloodGroup>(dto.BloodGroup);
            existingPatient.RhFactor = Enum.Parse<RhFactor>(dto.RhFactor);
            existingPatient.PhoneNumber = dto.PhoneNumber;
            
            await _patientService.UpdatePatientAsync(id, existingPatient);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Invalid enum value: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var existingPatient = await _patientService.GetPatientByIdAsync(id);
        if (existingPatient == null) 
        {
            return NotFound($"Patient with id {id} not found");
        }
        
        await _patientService.DeletePatientAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/appointments")]
    public async Task<ActionResult<List<AppointmentDto>>> GetPatientAppointments(int id)
    {
        var existingPatient = await _patientService.GetPatientByIdAsync(id);
        if (existingPatient == null) 
        {
            return NotFound($"Patient with id {id} not found");
        }

        var appointments = await _appointmentService.GetAppointmentsByPatientAsync(id);
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