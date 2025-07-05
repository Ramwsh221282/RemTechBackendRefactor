using Npgsql;
using RemTech.ParsersManagement.DataSource.Adapter.DataAccessConfiguration;

namespace RemTech.ParsersManagement.DataSource.Adapter.DataAccessObjects;

public sealed class PostgreSqlConnectionSource(ParsersManagementDatabaseConfiguration configuration)
{
    // public async Task<PostgreSqlConnection> CreateOpenedConnection()
    // {
    //     NpgsqlDataSource source = CreateDataSource();
    //     NpgsqlConnection connection = await source.OpenConnectionAsync();
    //     return new PostgreSqlConnection(source, connection, true);
    // }
    //
    // public async Task<PostgreSqlConnection> CreateOpenedConnection(CancellationToken ct)
    // {
    //     NpgsqlDataSource source = CreateDataSource();
    //     NpgsqlConnection connection = await source.OpenConnectionAsync(ct);
    //     return new PostgreSqlConnection(source, connection, true);
    // }
    //
    // public async Task<Transaction> CreateTransaction()
    // {
    //     PostgreSqlConnection connection = await CreateOpenedConnection();
    //     await connection.BeginTransactionAsync();
    //     return new Transaction(connection);
    // }
    //
    // public async Task<Transaction> CreateTransaction(CancellationToken ct)
    // {
    //     PostgreSqlConnection connection = await CreateOpenedConnection(ct);
    //     await connection.BeginTransactionAsync(ct);
    //     return new Transaction(connection);
    // }
    //
    // public NpgsqlDataSource CreateDataSource()
    // {
    //     string connectionString = configuration.ConnectionString;
    //     NpgsqlDataSourceBuilder builder = new(connectionString);
    //     NpgsqlDataSource source = builder.Build();
    //     return source;
    // }
}
