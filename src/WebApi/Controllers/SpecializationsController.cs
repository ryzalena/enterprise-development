﻿using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces;
using Application.Dtos;
using Domain.Entities;

namespace WebApi.Controllers;

/// <summary>
/// Контроллер для управления специализациями врачей
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class SpecializationsController(
    ISpecializationService service) : ControllerBase
{
    /// <summary>
    /// Получить все специализации
    /// </summary>
    /// <returns>Список всех специализаций</returns>
    [HttpGet]
    public async Task<ActionResult<List<SpecializationDto>>> GetSpecializations()
    {
        var specializations = await service.GetAllSpecializationsAsync();
        var dtos = specializations.Select(s => new SpecializationDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description ?? string.Empty
        }).ToList();

        return Ok(dtos);
    }

    /// <summary>
    /// Получить специализацию по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор специализации</param>
    /// <returns>Специализация</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<SpecializationDto>> GetSpecialization(int id)
    {
        var specialization = await service.GetSpecializationByIdAsync(id);
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

    /// <summary>
    /// Создать новую специализацию
    /// </summary>
    /// <param name="dto">Данные для создания специализации</param>
    /// <returns>Созданная специализация</returns>
    [HttpPost]
    public async Task<ActionResult<SpecializationDto>> CreateSpecialization(
        [FromBody] SpecializationManipulationDto dto)
    {
        var specialization = new Specialization
        {
            Id = 0,
            Name = dto.Name,
            Description = dto.Description
        };

        var created = await service.CreateSpecializationAsync(specialization);

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

    /// <summary>
    /// Обновить специализацию
    /// </summary>
    /// <param name="id">Идентификатор специализации</param>
    /// <param name="dto">Данные для обновления специализации</param>
    /// <returns>Результат операции</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpecialization(
        int id,
        [FromBody] SpecializationManipulationDto dto)
    {
        var existingSpecialization = await service.GetSpecializationByIdAsync(id);
        if (existingSpecialization == null)
        {
            return NotFound($"Specialization with id {id} not found");
        }

        existingSpecialization.Name = dto.Name;
        existingSpecialization.Description = dto.Description;

        await service.UpdateSpecializationAsync(id, existingSpecialization);
        return NoContent();
    }

    /// <summary>
    /// Удалить специализацию
    /// </summary>
    /// <param name="id">Идентификатор специализации</param>
    /// <returns>Результат операции</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpecialization(int id)
    {
        var existingSpecialization = await service.GetSpecializationByIdAsync(id);
        if (existingSpecialization == null)
        {
            return NoContent();
        }

        await service.DeleteSpecializationAsync(id);
        return NoContent();
    }
}