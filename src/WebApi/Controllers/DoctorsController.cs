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
    [HttpGet("{id}/appointments")]
    public async Task<ActionResult<List<AppointmentDto>>> GetDoctorAppointments(int id)
    {
        var doctor = await doctorService.GetDoctorByIdAsync(id);
        if (doctor == null)
        {
            return NotFound($"Doctor with id {id} not found");
        }

        var appointments = await appointmentService.GetAppointmentsByDoctorAsync(id);
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