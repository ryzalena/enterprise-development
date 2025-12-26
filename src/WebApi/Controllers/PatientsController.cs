using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;

namespace WebApi.Controllers;

/// <summary>
/// Контроллер для управления пациентами поликлиники
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PatientsController> _logger;

    /// <summary>
    /// Конструктор контроллера пациентов
    /// </summary>
    /// <param name="patientService">Сервис для работы с пациентами</param>
    /// <param name="context">Контекст базы данных</param>
    /// <param name="logger">Логгер</param>
    public PatientsController(
        IPatientService patientService,
        ApplicationDbContext context,
        ILogger<PatientsController> logger)
    {
        _patientService = patientService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Получить список всех пациентов
    /// </summary>
    /// <returns>Список всех пациентов</returns>
    /// <response code="200">Успешно возвращен список пациентов</response>
    /// <response code="500">Внутренняя ошибка сервера</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Patient>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<IEnumerable<Patient>>> GetAll()
    {
        try
        {
            _logger.LogInformation("Getting all patients");
            var patients = await _patientService.GetAllPatientsAsync();
            return Ok(patients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all patients");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Получить пациента по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пациента</param>
    /// <returns>Данные пациента</returns>
    /// <response code="200">Пациент найден</response>
    /// <response code="404">Пациент не найден</response>
    /// <response code="500">Внутренняя ошибка сервера</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Patient), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<Patient>> GetById(int id)
    {
        try
        {
            _logger.LogInformation("Getting patient with ID: {Id}", id);
            var patient = await _patientService.GetPatientByIdAsync(id);

            if (patient == null)
            {
                _logger.LogWarning("Patient with ID {Id} not found", id);
                return NotFound($"Patient with ID {id} not found");
            }

            return Ok(patient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patient with ID: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Создать нового пациента
    /// </summary>
    /// <param name="patient">Данные нового пациента</param>
    /// <returns>Созданный пациент</returns>
    /// <response code="201">Пациент успешно создан</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="500">Внутренняя ошибка сервера</response>
    [HttpPost]
    [ProducesResponseType(typeof(Patient), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<Patient>> Create([FromBody] Patient patient)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for patient creation");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating new patient: {FullName}", patient.FullName);
            var createdPatient = await _patientService.CreatePatientAsync(patient);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdPatient.Id },
                createdPatient);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while creating patient");
            return BadRequest("Error saving to database");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating patient");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Обновить данные пациента
    /// </summary>
    /// <param name="id">Идентификатор пациента</param>
    /// <param name="patient">Обновленные данные пациента</param>
    /// <returns>Результат операции</returns>
    /// <response code="204">Пациент успешно обновлен</response>
    /// <response code="400">Некорректные данные или ID</response>
    /// <response code="404">Пациент не найден</response>
    /// <response code="500">Внутренняя ошибка сервера</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    
    /// <summary>
    /// Обновить данные пациента
    /// </summary>
    public async Task<IActionResult> Update(int id, [FromBody] Patient patient)
    {
        try
        {
            if (id != patient.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating patient with ID: {Id}", id);
            
            await _patientService.UpdatePatientAsync(id, patient);
        
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Patient with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating patient");
            return StatusCode(500, "Internal server error");
        }
    }
    /// <summary>
    /// Удалить пациента
    /// </summary>
    /// <param name="id">Идентификатор пациента</param>
    /// <returns>Результат операции</returns>
    /// <response code="204">Пациент успешно удален</response>
    /// <response code="404">Пациент не найден</response>
    /// <response code="500">Внутренняя ошибка сервера</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInformation("Deleting patient with ID: {Id}", id);
            
            // Сначала проверим существование пациента
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
            {
                _logger.LogWarning("Patient with ID {Id} not found for deletion", id);
                return NotFound($"Patient with ID {id} not found");
            }

            await _patientService.DeletePatientAsync(id);
            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while deleting patient with ID: {Id}", id);
            return BadRequest("Cannot delete patient - possible foreign key constraint");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting patient with ID: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Поиск пациентов по имени
    /// </summary>
    /// <param name="name">Часть имени или фамилии пациента</param>
    /// <returns>Список найденных пациентов</returns>
    /// <response code="200">Успешно возвращен список пациентов</response>
    /// <response code="500">Внутренняя ошибка сервера</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<Patient>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<IEnumerable<Patient>>> Search([FromQuery] string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Search name cannot be empty");
            }

            _logger.LogInformation("Searching patients by name: {Name}", name);
            
            var patients = await _context.Patients
                .Where(p => p.FullName.Contains(name))
                .ToListAsync();

            return Ok(patients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching patients by name: {Name}", name);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Получить пациентов по возрастной группе
    /// </summary>
    /// <param name="minAge">Минимальный возраст</param>
    /// <param name="maxAge">Максимальный возраст</param>
    /// <returns>Список пациентов в возрастной группе</returns>
    /// <response code="200">Успешно возвращен список пациентов</response>
    /// <response code="500">Внутренняя ошибка сервера</response>
    [HttpGet("by-age")]
    [ProducesResponseType(typeof(IEnumerable<Patient>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<IEnumerable<Patient>>> GetByAge(
        [FromQuery] int? minAge = null,
        [FromQuery] int? maxAge = null)
    {
        try
        {
            _logger.LogInformation("Getting patients by age: {MinAge}-{MaxAge}", minAge, maxAge);
            
            var query = _context.Patients.AsQueryable();
            
            if (minAge.HasValue)
            {
                var minBirthDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-minAge.Value));
                query = query.Where(p => p.BirthDate <= minBirthDate);
            }
            
            if (maxAge.HasValue)
            {
                var maxBirthDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-maxAge.Value));
                query = query.Where(p => p.BirthDate >= maxBirthDate);
            }
            
            var patients = await query.ToListAsync();
            return Ok(patients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patients by age");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Получить статистику по пациентам
    /// </summary>
    /// <returns>Статистика пациентов</returns>
    /// <response code="200">Успешно возвращена статистика</response>
    /// <response code="500">Внутренняя ошибка сервера</response>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<object>> GetStatistics()
    {
        try
        {
            _logger.LogInformation("Getting patients statistics");
            
            var patients = await _context.Patients.ToListAsync();
            
            var ageGroups = patients
                .Select(p => DateTime.Today.Year - p.BirthDate.Year)
                .GroupBy(age => (age / 10) * 10) // Группируем по десяткам лет
                .Select(g => new
                {
                    AgeGroup = $"{g.Key}-{g.Key + 9} лет",
                    Count = g.Count()
                })
                .OrderBy(g => g.AgeGroup);

            var statistics = new
            {
                TotalPatients = patients.Count,
                AverageAge = patients.Any() 
                    ? patients.Average(p => DateTime.Today.Year - p.BirthDate.Year) 
                    : 0,
                GenderDistribution = patients
                    .GroupBy(p => p.Gender)
                    .Select(g => new
                    {
                        Gender = g.Key.ToString(),
                        Count = g.Count(),
                        Percentage = patients.Any() 
                            ? (double)g.Count() / patients.Count * 100 
                            : 0
                    }),
                AgeGroups = ageGroups
            };

            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patients statistics");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Получить пациентов с количеством записей на прием
    /// </summary>
    /// <returns>Список пациентов с количеством записей</returns>
    /// <response code="200">Успешно возвращен список</response>
    /// <response code="500">Внутренняя ошибка сервера</response>
    [HttpGet("with-appointments-count")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<IEnumerable<object>>> GetWithAppointmentsCount()
    {
        try
        {
            _logger.LogInformation("Getting patients with appointments count");
            
            var patientsWithCount = await _context.Patients
                .Include(p => p.Appointments)
                .Select(p => new
                {
                    p.Id,
                    p.FullName,
                    p.Age,
                    p.Gender,
                    AppointmentsCount = p.Appointments.Count,
                    LastAppointment = p.Appointments
                        .OrderByDescending(a => a.AppointmentDateTime)
                        .FirstOrDefault()
                })
                .OrderByDescending(p => p.AppointmentsCount)
                .ToListAsync();

            return Ok(patientsWithCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patients with appointments count");
            return StatusCode(500, "Internal server error");
        }
    }
}