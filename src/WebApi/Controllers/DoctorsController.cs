using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;

namespace WebApi.Controllers;

/// <summary>
/// Контроллер для управления врачами поликлиники
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DoctorsController> _logger;

    /// <summary>
    /// Конструктор контроллера врачей
    /// </summary>
    public DoctorsController(
        IDoctorService doctorService,
        ApplicationDbContext context,
        ILogger<DoctorsController> logger)
    {
        _doctorService = doctorService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Получить список всех врачей
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Doctor>), 200)]
    public async Task<ActionResult<IEnumerable<Doctor>>> GetAll()
    {
        try
        {
            _logger.LogInformation("Getting all doctors");
            var doctors = await _doctorService.GetAllDoctorsAsync();
            return Ok(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all doctors");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Получить врача по идентификатору
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Doctor), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Doctor>> GetById(int id)
    {
        try
        {
            _logger.LogInformation("Getting doctor with ID: {Id}", id);
            var doctor = await _doctorService.GetDoctorByIdAsync(id);

            if (doctor == null)
            {
                return NotFound($"Doctor with ID {id} not found");
            }

            return Ok(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctor with ID: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Создать нового врача
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Doctor), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Doctor>> Create([FromBody] Doctor doctor)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating new doctor: {FullName}", doctor.FullName);
            var createdDoctor = await _doctorService.CreateDoctorAsync(doctor);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdDoctor.Id },
                createdDoctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating doctor");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Обновить данные врача
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] Doctor doctor)
    {
        try
        {
            if (id != doctor.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating doctor with ID: {Id}", id);
        
            await _doctorService.UpdateDoctorAsync(id, doctor);
        
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating doctor");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Удалить врача
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInformation("Deleting doctor with ID: {Id}", id);
            
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
            {
                return NotFound($"Doctor with ID {id} not found");
            }

            await _doctorService.DeleteDoctorAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor with ID: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Получить врачей по специализации
    /// </summary>
    [HttpGet("by-specialization/{specializationId:int}")]
    [ProducesResponseType(typeof(IEnumerable<Doctor>), 200)]
    public async Task<ActionResult<IEnumerable<Doctor>>> GetBySpecialization(int specializationId)
    {
        try
        {
            _logger.LogInformation("Getting doctors by specialization ID: {SpecializationId}", specializationId);
            
            var doctors = await _context.Doctors
                .Where(d => d.SpecializationId == specializationId)
                .Include(d => d.Specialization)
                .ToListAsync();
                
            return Ok(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctors by specialization ID: {SpecializationId}", specializationId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Получить врачей со стажем больше указанного
    /// </summary>
    [HttpGet("experienced/{minExperience:int}")]
    [ProducesResponseType(typeof(IEnumerable<Doctor>), 200)]
    public async Task<ActionResult<IEnumerable<Doctor>>> GetExperienced(int minExperience)
    {
        try
        {
            _logger.LogInformation("Getting doctors with experience >= {MinExperience} years", minExperience);
            
            var doctors = await _context.Doctors
                .Where(d => d.ExperienceYears >= minExperience)
                .Include(d => d.Specialization)
                .OrderByDescending(d => d.ExperienceYears)
                .ToListAsync();
                
            return Ok(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctors with experience >= {MinExperience}", minExperience);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Поиск врачей по имени
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<Doctor>), 200)]
    public async Task<ActionResult<IEnumerable<Doctor>>> Search([FromQuery] string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Search name cannot be empty");
            }

            _logger.LogInformation("Searching doctors by name: {Name}", name);
            
            var doctors = await _context.Doctors
                .Where(d => d.FullName.Contains(name))
                .Include(d => d.Specialization)
                .ToListAsync();

            return Ok(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching doctors by name: {Name}", name);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Получить статистику по врачам
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<ActionResult<object>> GetStatistics()
    {
        try
        {
            _logger.LogInformation("Getting doctors statistics");
            
            var doctors = await _context.Doctors
                .Include(d => d.Specialization)
                .ToListAsync();

            var statistics = new
            {
                TotalDoctors = doctors.Count,
                AverageExperience = doctors.Any() 
                    ? doctors.Average(d => d.ExperienceYears) 
                    : 0,
                MaxExperience = doctors.Any() 
                    ? doctors.Max(d => d.ExperienceYears) 
                    : 0,
                MinExperience = doctors.Any() 
                    ? doctors.Min(d => d.ExperienceYears) 
                    : 0,
                BySpecialization = doctors
                    .GroupBy(d => d.Specialization != null ? d.Specialization.Name : "Unknown")
                    .Select(g => new
                    {
                        Specialization = g.Key,
                        Count = g.Count()
                    })
            };

            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctors statistics");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Получить врачей с количеством записей на прием
    /// </summary>
    [HttpGet("with-appointments-count")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<ActionResult<IEnumerable<object>>> GetWithAppointmentsCount()
    {
        try
        {
            _logger.LogInformation("Getting doctors with appointments count");
            
            var doctorsWithCount = await _context.Doctors
                .Include(d => d.Appointments)
                .Select(d => new
                {
                    d.Id,
                    d.FullName,
                    d.ExperienceYears,
                    Specialization = d.Specialization != null ? d.Specialization.Name : "Unknown",
                    AppointmentsCount = d.Appointments.Count
                })
                .OrderByDescending(d => d.AppointmentsCount)
                .ToListAsync();

            return Ok(doctorsWithCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctors with appointments count");
            return StatusCode(500, "Internal server error");
        }
    }
}