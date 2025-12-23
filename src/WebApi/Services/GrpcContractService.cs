using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebApi.Services;

public class GrpcContractService // Убрал наследование, т.к. ContractServiceBase не существует
{
    private readonly ILogger<GrpcContractService> _logger;
    private readonly string _connectionString;

    public GrpcContractService(ILogger<GrpcContractService> logger, IConfiguration configuration)
    {
        _logger = logger;
        // Используем "clinicdb" вместо "DefaultConnection" и правильный пароль
        _connectionString = configuration.GetConnectionString("clinicdb")
            ?? "Server=localhost,1433;Database=clinicdb;User Id=sa;Password=MySecurePassword123!;TrustServerCertificate=True;";
        
        _logger.LogInformation("GrpcContractService initialized");
    }

    public async Task<int> GetContractsCountAsync()
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand("SELECT COUNT(*) FROM MedicalContracts", connection);
            var count = (int)await command.ExecuteScalarAsync();
            
            _logger.LogInformation("Total contracts in database: {Count}", count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contracts count");
            throw;
        }
    }

    public async Task<List<MedicalContract>> GetContractsAsync(int page = 0, int pageSize = 10)
    {
        var contracts = new List<MedicalContract>();

        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

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

            while (await reader.ReadAsync())
            {
                var contract = new MedicalContract
                {
                    Id = reader["Id"].ToString() ?? string.Empty,
                    PatientId = reader["PatientId"].ToString() ?? string.Empty,
                    DoctorId = reader["DoctorId"].ToString() ?? string.Empty,
                    ServiceType = reader["ServiceType"].ToString() ?? string.Empty,
                    Price = reader["Price"] as decimal? ?? 0,
                    Status = reader["Status"].ToString() ?? string.Empty,
                    CreatedDate = reader["CreatedDate"] as DateTime? ?? DateTime.MinValue,
                    ValidUntil = reader["ValidUntil"] as DateTime? ?? DateTime.MinValue,
                    AppointmentId = reader["AppointmentId"]?.ToString(),
                    Diagnosis = reader["Diagnosis"]?.ToString(),
                    PrescribedMedicationsJson = reader["PrescribedMedicationsJson"]?.ToString(),
                    TreatmentPlan = reader["TreatmentPlan"]?.ToString(),
                    GeneratorId = reader["GeneratorId"]?.ToString() ?? string.Empty,
                    ReceivedAt = reader["ReceivedAt"] as DateTime? ?? DateTime.MinValue,
                    Source = reader["Source"].ToString() ?? string.Empty
                };

                contracts.Add(contract);
            }
            
            _logger.LogInformation("Retrieved {Count} contracts", contracts.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contracts");
            throw;
        }

        return contracts;
    }

    public async Task<MedicalContract?> GetContractAsync(string id)
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);
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
                return new MedicalContract
                {
                    Id = reader["Id"].ToString() ?? string.Empty,
                    PatientId = reader["PatientId"].ToString() ?? string.Empty,
                    DoctorId = reader["DoctorId"].ToString() ?? string.Empty,
                    ServiceType = reader["ServiceType"].ToString() ?? string.Empty,
                    Price = reader["Price"] as decimal? ?? 0,
                    Status = reader["Status"].ToString() ?? string.Empty,
                    CreatedDate = reader["CreatedDate"] as DateTime? ?? DateTime.MinValue,
                    ValidUntil = reader["ValidUntil"] as DateTime? ?? DateTime.MinValue,
                    AppointmentId = reader["AppointmentId"]?.ToString(),
                    Diagnosis = reader["Diagnosis"]?.ToString(),
                    PrescribedMedicationsJson = reader["PrescribedMedicationsJson"]?.ToString(),
                    TreatmentPlan = reader["TreatmentPlan"]?.ToString(),
                    GeneratorId = reader["GeneratorId"]?.ToString() ?? string.Empty,
                    ReceivedAt = reader["ReceivedAt"] as DateTime? ?? DateTime.MinValue,
                    Source = reader["Source"].ToString() ?? string.Empty
                };
            }
            
            _logger.LogWarning("Contract {Id} not found", id);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contract {Id}", id);
            throw;
        }
    }
}

public class MedicalContract
{
    public string Id { get; set; } = string.Empty;
    public string PatientId { get; set; } = string.Empty;
    public string DoctorId { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime ValidUntil { get; set; }
    public string? AppointmentId { get; set; }
    public string? Diagnosis { get; set; }
    public string? PrescribedMedicationsJson { get; set; }
    public string? TreatmentPlan { get; set; }
    public string GeneratorId { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
    public string Source { get; set; } = string.Empty;
}