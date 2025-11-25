using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.Dtos;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpecializationsController : ControllerBase
{
    private readonly IPolyclinicService _service;

    public SpecializationsController(IPolyclinicService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<SpecializationDto>>> GetSpecializations()
    {
        var specializations = await _service.GetSpecializationsAsync();
        var dtos = specializations.Select(s => new SpecializationDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description
        }).ToList();
        
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SpecializationDto>> GetSpecialization(int id)
    {
        var specialization = await _service.GetSpecializationByIdAsync(id);
        if (specialization == null) return NotFound();
        
        var dto = new SpecializationDto
        {
            Id = specialization.Id,
            Name = specialization.Name,
            Description = specialization.Description
        };
        
        return Ok(dto);
    }
}