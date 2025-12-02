using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces;
using Application.Dtos;
using Domain.Entities;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class SpecializationsController : ControllerBase
{
    private readonly ISpecializationService _service;

    public SpecializationsController(ISpecializationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<SpecializationDto>>> GetSpecializations()
    {
        var specializations = await _service.GetAllSpecializationsAsync();
        var dtos = specializations.Select(s => new SpecializationDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description ?? string.Empty
        }).ToList();
        
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SpecializationDto>> GetSpecialization(int id)
    {
        var specialization = await _service.GetSpecializationByIdAsync(id);
        if (specialization == null) 
        {
            return NotFound($"Specialization with id {id} not found");
        }
        
        var dto = new SpecializationDto
        {
            Id = specialization.Id,
            Name = specialization.Name,
            Description = specialization.Description ?? string.Empty
        };
        
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<SpecializationDto>> CreateSpecialization(
        [FromBody] SpecializationManipulationDto dto)
    {
        var specialization = new Specialization
        {
            Id = 0, // Будет сгенерировано сервисом
            Name = dto.Name,
            Description = dto.Description
        };
        
        var created = await _service.CreateSpecializationAsync(specialization);
        
        var resultDto = new SpecializationDto
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description ?? string.Empty
        };
        
        return CreatedAtAction(
            nameof(GetSpecialization), 
            new { id = created.Id }, 
            resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpecialization(
        int id, 
        [FromBody] SpecializationManipulationDto dto)
    {
        var existingSpecialization = await _service.GetSpecializationByIdAsync(id);
        if (existingSpecialization == null) 
        {
            return NotFound($"Specialization with id {id} not found");
        }

        existingSpecialization.Name = dto.Name;
        existingSpecialization.Description = dto.Description;
        
        await _service.UpdateSpecializationAsync(id, existingSpecialization);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpecialization(int id)
    {
        var existingSpecialization = await _service.GetSpecializationByIdAsync(id);
        if (existingSpecialization == null) 
        {
            return NotFound($"Specialization with id {id} not found");
        }
        
        await _service.DeleteSpecializationAsync(id);
        return NoContent();
    }
}