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
    [HttpGet]
    public async Task<ActionResult<List<AppointmentDto>>> GetAppointments()
    {
        var appointments = await appointmentService.GetAllAppointmentsAsync();
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

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDto>> GetAppointment(int id)
    {
        var appointment = await appointmentService.GetAppointmentByIdAsync(id);
        if (appointment == null) return NotFound();

        var dto = new AppointmentDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            AppointmentDateTime = appointment.AppointmentDateTime,
            RoomNumber = appointment.RoomNumber,
            IsFollowUp = appointment.IsFollowUp,
            Patient = appointment.Patient != null ? new PatientDto
            {
                Id = appointment.Patient.Id,
                PassportNumber = appointment.Patient.PassportNumber,
                FullName = appointment.Patient.FullName,
                Gender = appointment.Patient.Gender.ToString(),
                BirthDate = appointment.Patient.BirthDate,
                Address = appointment.Patient.Address,
                BloodGroup = appointment.Patient.BloodGroup.ToString(),
                RhFactor = appointment.Patient.RhFactor.ToString(),
                PhoneNumber = appointment.Patient.PhoneNumber
            } : null,
            Doctor = appointment.Doctor != null ? new DoctorDto
            {
                Id = appointment.Doctor.Id,
                PassportNumber = appointment.Doctor.PassportNumber,
                FullName = appointment.Doctor.FullName,
                BirthYear = appointment.Doctor.BirthYear,
                SpecializationName = appointment.Doctor.Specialization?.Name ?? string.Empty,
                ExperienceYears = appointment.Doctor.ExperienceYears
            } : null
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> CreateAppointment(
        [FromBody] AppointmentManipulationDto dto)
    {
        var patient = await patientService.GetPatientByIdAsync(dto.PatientId);
        var doctor = await doctorService.GetDoctorByIdAsync(dto.DoctorId);

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

        var createdAppointment = await appointmentService.CreateAppointmentAsync(appointment);

        var resultDto = new AppointmentDto
        {
            Id = createdAppointment.Id,
            PatientId = createdAppointment.PatientId,
            DoctorId = createdAppointment.DoctorId,
            AppointmentDateTime = createdAppointment.AppointmentDateTime,
            RoomNumber = createdAppointment.RoomNumber,
            IsFollowUp = createdAppointment.IsFollowUp,
            Patient = patient != null ? new PatientDto
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
            } : null,
            Doctor = doctor != null ? new DoctorDto
            {
                Id = doctor.Id,
                PassportNumber = doctor.PassportNumber,
                FullName = doctor.FullName,
                BirthYear = doctor.BirthYear,
                SpecializationName = doctor.Specialization?.Name ?? string.Empty,
                ExperienceYears = doctor.ExperienceYears
            } : null
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
        var existingAppointment = await appointmentService.GetAppointmentByIdAsync(id);
        if (existingAppointment == null)
        {
            return NotFound($"Appointment with id {id} not found");
        }

        var patient = await patientService.GetPatientByIdAsync(dto.PatientId);
        var doctor = await doctorService.GetDoctorByIdAsync(dto.DoctorId);

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

        await appointmentService.UpdateAppointmentAsync(id, existingAppointment);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var existingAppointment = await appointmentService.GetAppointmentByIdAsync(id);
        if (existingAppointment == null)
        {
            return NoContent();
        }

        await appointmentService.DeleteAppointmentAsync(id);

        return NoContent();
    }
}