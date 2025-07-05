using Dapper;
using DbUp;
using DbUp.Engine;
using RemTech.ParsersManagement.DataSource.Adapter.DataAccessObjects;

namespace RemTech.ParsersManagement.DataSource.Adapter.DataAccessConfiguration;

public static class ParsersManagementDbUp
{
    // public static void CreateDatabase(this ParsersManagementDatabaseConfiguration configuration)
    // {
    //     string connectionString = configuration.ConnectionString;
    //     EnsureDatabase.For.PostgresqlDatabase(connectionString);
    //     UpgradeEngine upgrader = DeployChanges
    //         .To.PostgresqlDatabase(connectionString)
    //         .WithScriptsEmbeddedInAssembly(typeof(ParsersManagementDbUp).Assembly)
    //         .LogToConsole()
    //         .Build();
    //     DatabaseUpgradeResult result = upgrader.PerformUpgrade();
    //     if (!result.Successful)
    //         throw new ApplicationException("Failed to create parsers management database.");
    // }
    //
    // public static async Task DropTables(this PostgreSqlConnectionSource connectionSource)
    // {
    //     await using PostgreSqlConnection connection =
    //         await connectionSource.CreateOpenedConnection();
    //     const string sql = "DROP TABLE IF EXISTS parsers, parser_links";
    //     await connection.ExecuteAsync(sql);
    // }
}
