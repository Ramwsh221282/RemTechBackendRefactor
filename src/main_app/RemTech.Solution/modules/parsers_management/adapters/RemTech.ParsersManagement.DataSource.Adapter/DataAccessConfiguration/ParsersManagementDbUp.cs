using Dapper;
using DbUp;
using DbUp.Engine;
using Npgsql;

namespace RemTech.ParsersManagement.DataSource.Adapter.DataAccessConfiguration;

public sealed class ParsersManagementDbUp
{
    private readonly ParsersManagementDatabaseConfiguration _configuration;

    public ParsersManagementDbUp(ParsersManagementDatabaseConfiguration configuration) =>
        _configuration = configuration;

    public void Up()
    {
        string connectionString = _configuration.ConnectionString;
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(ParsersManagementDbUp).Assembly)
            .LogToConsole()
            .Build();
        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new ApplicationException("Failed to create parsers management database.");
    }

    public async Task Down()
    {
        await using PostgreSqlEngine engine = new(_configuration);
        NpgsqlConnection connection = await engine.Connect();
        await connection.ExecuteAsync("DROP TABLE IF EXISTS parsers, parser_links");
        await connection.ExecuteAsync("DROP TABLE IF EXISTS schemaversions");
    }
}
