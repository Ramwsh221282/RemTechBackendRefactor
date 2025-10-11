namespace Telemetry.Infrastructure.PostgreSQL;

public sealed class PostgreSqlConnectionOptions
{
    public required string Hostname { get; set; }
    public required string Port { get; set; }
    public required string Database { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }

    public string FormConnectionString()
    {
        return $"Host={Hostname};Port={Port};Database={Database};Username={Username};Password={Password};";
    }
}
