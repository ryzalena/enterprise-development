using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using NATS.Client;
using System.Text.Json;

namespace WebApi.Services;

public class SimpleNatsService : BackgroundService
{
    private readonly ILogger<SimpleNatsService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private IConnection? _natsConnection;

    public SimpleNatsService(
        ILogger<SimpleNatsService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _connectionString = configuration.GetConnectionString("clinicdb") 
            ?? "Server=localhost,1433;Database=clinicdb;User Id=sa;Password=MySecurePassword123!;TrustServerCertificate=True;";
        
        _logger.LogInformation("NATS Service initialized.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var natsUrl = _configuration["Nats:Url"] ?? "nats://localhost:4222";
        var subject = _configuration["Nats:ContractSubject"] ?? "polyclinic.contracts.generated";

        _logger.LogInformation("Starting NATS service for subject: {Subject}", subject);

        try
        {
            var opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = natsUrl;
            opts.AllowReconnect = true;
            opts.MaxReconnect = Options.ReconnectForever;
            opts.ReconnectWait = 1000;
            
            _natsConnection = new ConnectionFactory().CreateConnection(opts);
            
            _logger.LogInformation("Connected to NATS at {Url}", natsUrl);

            var subscription = _natsConnection.SubscribeAsync(subject, (sender, args) =>
            {
                try
                {
                    var messageText = System.Text.Encoding.UTF8.GetString(args.Message.Data);
                    _ = Task.Run(() => ProcessMessageAsync(messageText));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing NATS message");
                }
            });

            _logger.LogInformation("Subscribed to NATS subject: {Subject}", subject);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
                
                if (_natsConnection.State != ConnState.CONNECTED)
                {
                    _logger.LogWarning("NATS connection lost. State: {State}", _natsConnection.State);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "NATS service failed");
        }
        finally
        {
            if (_natsConnection != null && _natsConnection.State == ConnState.CONNECTED)
            {
                _natsConnection.Close();
                _logger.LogInformation("NATS connection closed");
            }
        }
    }

    private async Task ProcessMessageAsync(string json)
    {
        try
        {
            _logger.LogDebug("Received NATS message: {Length} bytes", json.Length);

            var contractData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            if (contractData == null)
            {
                _logger.LogWarning("Invalid JSON received from NATS");
                return;
            }

            var contractId = GetStringValue(contractData, "Id");
            if (string.IsNullOrEmpty(contractId))
            {
                _logger.LogWarning("Contract ID is missing in NATS message");
                return;
            }

            _logger.LogInformation("Processing contract {ContractId} from NATS", contractId);

            await SaveToDatabaseAsync(contractData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing NATS message");
        }
    }

    private string GetStringValue(Dictionary<string, JsonElement> data, string key)
    {
        if (data.TryGetValue(key, out var element) && element.ValueKind == JsonValueKind.String)
        {
            return element.GetString() ?? string.Empty;
        }
        return string.Empty;
    }

    private decimal GetDecimalValue(Dictionary<string, JsonElement> data, string key, decimal defaultValue = 0)
    {
        if (data.TryGetValue(key, out var element))
        {
            try
            {
                if (element.ValueKind == JsonValueKind.Number)
                {
                    return element.GetDecimal();
                }
                else if (element.ValueKind == JsonValueKind.String && decimal.TryParse(element.GetString(), out var result))
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error parsing decimal value for key {Key}", key);
            }
        }
        return defaultValue;
    }

    private DateTime GetDateTimeValue(Dictionary<string, JsonElement> data, string key, DateTime defaultValue)
    {
        if (data.TryGetValue(key, out var element))
        {
            try
            {
                if (element.ValueKind == JsonValueKind.String)
                {
                    if (DateTime.TryParse(element.GetString(), out var result))
                    {
                        return result;
                    }
                }
                else if (element.ValueKind == JsonValueKind.Number)
                {
                    // Предполагаем что это Unix timestamp
                    var timestamp = element.GetInt64();
                    return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error parsing DateTime value for key {Key}", key);
            }
        }
        return defaultValue;
    }

    private async Task SaveToDatabaseAsync(Dictionary<string, JsonElement> data)
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var contractId = GetStringValue(data, "Id");
            if (string.IsNullOrEmpty(contractId))
            {
                _logger.LogWarning("Contract ID is empty");
                return;
            }

            // Проверяем существование
            await using var checkCommand = new SqlCommand(
                "SELECT COUNT(*) FROM MedicalContracts WHERE Id = @Id", connection);
            checkCommand.Parameters.AddWithValue("@Id", contractId);

            var exists = (int)await checkCommand.ExecuteScalarAsync() > 0;
            if (exists)
            {
                _logger.LogDebug("Contract {ContractId} already exists", contractId);
                return;
            }

            // Извлекаем данные с помощью новых методов
            var patientId = GetStringValue(data, "PatientId");
            var doctorId = GetStringValue(data, "DoctorId");
            var serviceType = GetStringValue(data, "ServiceType");
            if (string.IsNullOrEmpty(serviceType)) serviceType = "Не указано";
            
            var price = GetDecimalValue(data, "Price");
            var status = GetStringValue(data, "Status");
            if (string.IsNullOrEmpty(status)) status = "Создан";
            
            var createdDate = GetDateTimeValue(data, "CreatedDate", DateTime.UtcNow);
            var validUntil = GetDateTimeValue(data, "ValidUntil", DateTime.UtcNow.AddDays(30));
            
            var appointmentId = GetStringValue(data, "AppointmentId");
            var diagnosis = GetStringValue(data, "Diagnosis");
            var treatmentPlan = GetStringValue(data, "TreatmentPlan");
            var generatorId = GetStringValue(data, "GeneratorId");
            if (string.IsNullOrEmpty(generatorId)) generatorId = "NATS-Generator";

            // Обрабатываем лекарства
            var medicationsJson = "[]";
            if (data.TryGetValue("PrescribedMedications", out var medsElement) && 
                medsElement.ValueKind == JsonValueKind.Array)
            {
                var medications = new List<string>();
                foreach (var item in medsElement.EnumerateArray())
                {
                    medications.Add(item.ToString());
                }
                medicationsJson = JsonSerializer.Serialize(medications);
            }

            // Вставляем контракт
            await using var insertCommand = new SqlCommand(@"
                INSERT INTO MedicalContracts 
                (Id, PatientId, DoctorId, ServiceType, Price, Status, 
                 CreatedDate, ValidUntil, AppointmentId, Diagnosis, 
                 PrescribedMedicationsJson, TreatmentPlan, GeneratorId, 
                 ReceivedAt, Source)
                VALUES 
                (@Id, @PatientId, @DoctorId, @ServiceType, @Price, @Status, 
                 @CreatedDate, @ValidUntil, @AppointmentId, @Diagnosis, 
                 @MedicationsJson, @TreatmentPlan, @GeneratorId, 
                 @ReceivedAt, @Source)", connection);
            
            insertCommand.Parameters.AddWithValue("@Id", contractId);
            insertCommand.Parameters.AddWithValue("@PatientId", patientId);
            insertCommand.Parameters.AddWithValue("@DoctorId", doctorId);
            insertCommand.Parameters.AddWithValue("@ServiceType", serviceType);
            insertCommand.Parameters.AddWithValue("@Price", price);
            insertCommand.Parameters.AddWithValue("@Status", status);
            insertCommand.Parameters.AddWithValue("@CreatedDate", createdDate);
            insertCommand.Parameters.AddWithValue("@ValidUntil", validUntil);
            insertCommand.Parameters.AddWithValue("@AppointmentId", string.IsNullOrEmpty(appointmentId) ? (object)DBNull.Value : appointmentId);
            insertCommand.Parameters.AddWithValue("@Diagnosis", string.IsNullOrEmpty(diagnosis) ? (object)DBNull.Value : diagnosis);
            insertCommand.Parameters.AddWithValue("@MedicationsJson", medicationsJson);
            insertCommand.Parameters.AddWithValue("@TreatmentPlan", string.IsNullOrEmpty(treatmentPlan) ? (object)DBNull.Value : treatmentPlan);
            insertCommand.Parameters.AddWithValue("@GeneratorId", generatorId);
            insertCommand.Parameters.AddWithValue("@ReceivedAt", DateTime.UtcNow);
            insertCommand.Parameters.AddWithValue("@Source", "NATS");

            var rowsAffected = await insertCommand.ExecuteNonQueryAsync();
            
            _logger.LogInformation("Contract {ContractId} saved to database from NATS", contractId);
        }
        catch (SqlException sqlEx)
        {
            _logger.LogError(sqlEx, "SQL Error saving contract from NATS to database");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving contract from NATS to database");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping NATS service");
        
        if (_natsConnection != null)
        {
            try
            {
                if (_natsConnection.State == ConnState.CONNECTED)
                {
                    _natsConnection.Close();
                }
                _natsConnection.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing NATS connection");
            }
        }
        
        await base.StopAsync(cancellationToken);
        _logger.LogInformation("NATS service stopped");
    }
}