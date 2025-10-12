namespace RemTech.Infrastructure.PostgreSQL;

public sealed class NpgsqlOptions
{
    public required string Hostname { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string Port { get; init; }
    public required string Database { get; init; }

    public string FormConnectionString()
    {
        return $"Host={Hostname};Port={Port};Database={Database};Username={Username};Password={Password};";
    }
}
