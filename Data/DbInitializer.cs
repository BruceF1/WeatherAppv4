using WeatherAPPV4.Controllers;

public class DbInitializer
{
    private readonly ILogger<DbInitializer> _logger;
    private readonly string _masterConnectionString;
    private readonly string _dbConnectionString;


    public DbInitializer(IConfiguration configuration, ILogger<DbInitializer> logger)
    {
        _masterConnectionString = configuration.GetConnectionString("MasterDatabase");
        _dbConnectionString = configuration.GetConnectionString("AppDatabase");
        _logger = logger;
    }

    public void Initialize()
    {
        _logger.LogInformation("Initializing database...");
        CreateDatabaseIfNotExists();
        CreateTablesIfNotExists();
        _logger.LogInformation("Database initialization completed successfully.");
    }

    private void CreateDatabaseIfNotExists()
    {
        try
        {
            using var connection = new SqlConnection(_masterConnectionString);
            connection.Open();
            var dbExists = connection.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM sys.databases WHERE name = @DbName",
                new { DbName = "WeatherAPP4" });
            if (dbExists == 0)
            {
                connection.Execute("CREATE DATABASE WeatherAPP4");
                _logger.LogInformation("New DB Created");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error during DB initialization check", ex);
            throw;
        }

    }

    private void CreateTablesIfNotExists()
    {
        using (var connection = new SqlConnection(_dbConnectionString))
        {
            try
            {
                connection.Open();

                var createTableQuery = @"
IF NOT EXISTS (
		SELECT *
		FROM INFORMATION_SCHEMA.TABLES
		WHERE TABLE_NAME = 'Weather'
		)
	CREATE TABLE Weather (
		EntryDate DATETIME
		,AirTemperature FLOAT
		,SwellDirection FLOAT
		,SwellHeight FLOAT
		,SwellPeriod FLOAT
		,WaterTemperature FLOAT
		,WindDirection FLOAT
		,WindSpeed FLOAT
		)
                ;";

                connection.Execute(createTableQuery);
                _logger.LogInformation("Weather table created");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during table creation", ex);
                throw;
            }
        }
    } 
}
