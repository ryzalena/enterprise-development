﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace WebApi.Controllers;

/// <summary>
/// Контроллер для управления медицинскими контрактами
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ContractsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ContractsController> _logger;

    /// <summary>
    /// Инициализирует новый экземпляр контроллера контрактов
    /// </summary>
    /// <param name="configuration">Конфигурация приложения</param>
    /// <param name="logger">Логгер</param>
    public ContractsController(
        IConfiguration configuration,
        ILogger<ContractsController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Получить список контрактов с пагинацией
    /// </summary>
    /// <param name="page">Номер страницы (начиная с 0)</param>
    /// <param name="pageSize">Размер страницы</param>
    /// <returns>Страница контрактов</returns>
    [HttpGet]
    public async Task<IActionResult> GetContracts(
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("clinicdb")
                ?? "Server=localhost,1433;Database=clinicdb;User Id=sa;Password=MySecurePassword123!;TrustServerCertificate=True;";

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Получаем общее количество
            await using var countCommand = new SqlCommand("SELECT COUNT(*) FROM MedicalContracts", connection);
            var totalCount = (int)await countCommand.ExecuteScalarAsync();

            // Получаем контракты
            var query = @"
                SELECT 
                    Id, PatientId, DoctorId, ServiceType, Price, Status, 
                    CreatedDate, ValidUntil, AppointmentId, Diagnosis,
                    PrescribedMedicationsJson, TreatmentPlan, GeneratorId,
                    ReceivedAt, Source
                FROM MedicalContracts
                ORDER BY ReceivedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            await using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Offset", page * pageSize);
            command.Parameters.AddWithValue("@PageSize", pageSize);

            await using var reader = await command.ExecuteReaderAsync();
            var contracts = new List<object>();

            while (await reader.ReadAsync())
            {
                contracts.Add(new
                {
                    Id = reader["Id"] as string,
                    PatientId = reader["PatientId"] as string,
                    DoctorId = reader["DoctorId"] as string,
                    ServiceType = reader["ServiceType"] as string,
                    Price = reader["Price"] as decimal? ?? 0,
                    Status = reader["Status"] as string,
                    CreatedDate = reader["CreatedDate"] as DateTime?,
                    ValidUntil = reader["ValidUntil"] as DateTime?,
                    AppointmentId = reader["AppointmentId"] as string,
                    Diagnosis = reader["Diagnosis"] as string,
                    PrescribedMedicationsJson = reader["PrescribedMedicationsJson"] as string,
                    TreatmentPlan = reader["TreatmentPlan"] as string,
                    GeneratorId = reader["GeneratorId"] as string,
                    ReceivedAt = reader["ReceivedAt"] as DateTime?,
                    Source = reader["Source"] as string
                });
            }

            _logger.LogInformation("Returning {Count} contracts", contracts.Count);

            return Ok(new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Data = contracts
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contracts");
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    /// <summary>
    /// Получить контракт по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор контракта</param>
    /// <returns>Контракт</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetContract(string id)
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("clinicdb")
                ?? "Server=localhost,1433;Database=clinicdb;User Id=sa;Password=MySecurePassword123!;TrustServerCertificate=True;";

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT 
                    Id, PatientId, DoctorId, ServiceType, Price, Status, 
                    CreatedDate, ValidUntil, AppointmentId, Diagnosis,
                    PrescribedMedicationsJson, TreatmentPlan, GeneratorId,
                    ReceivedAt, Source
                FROM MedicalContracts
                WHERE Id = @Id";

            await using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var contract = new
                {
                    Id = reader["Id"] as string,
                    PatientId = reader["PatientId"] as string,
                    DoctorId = reader["DoctorId"] as string,
                    ServiceType = reader["ServiceType"] as string,
                    Price = reader["Price"] as decimal? ?? 0,
                    Status = reader["Status"] as string,
                    CreatedDate = reader["CreatedDate"] as DateTime?,
                    ValidUntil = reader["ValidUntil"] as DateTime?,
                    AppointmentId = reader["AppointmentId"] as string,
                    Diagnosis = reader["Diagnosis"] as string,
                    PrescribedMedicationsJson = reader["PrescribedMedicationsJson"] as string,
                    TreatmentPlan = reader["TreatmentPlan"] as string,
                    GeneratorId = reader["GeneratorId"] as string,
                    ReceivedAt = reader["ReceivedAt"] as DateTime?,
                    Source = reader["Source"] as string
                };

                _logger.LogInformation("Returning contract {Id}", id);
                return Ok(contract);
            }

            return NotFound(new { Error = $"Contract with id '{id}' not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contract {Id}", id);
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }
}