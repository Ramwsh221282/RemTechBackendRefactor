namespace RemTech.SharedKernel.Configurations;

public sealed class NpgSqlOptions
{
    public string Host { get; set; } = null!;
    public string Port { get; set; } = null!;
    public string Database { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string ToConnectionString()
    {
        ValidateOptions();
        return CreateFromTemplate();
    }

    private string CreateFromTemplate()
    {
        const string template = "Host={0};Port={1};Database={2};Username={3};Password={4};";
        return string.Format(template, Host, Port, Database, Username, Password);
    }
    
    private void ValidateOptions()
    {
        if (string.IsNullOrWhiteSpace(Host))
            throw new ArgumentException("Cannot use NpgSql Options. Host was not set.");
        if (string.IsNullOrWhiteSpace(Port))
            throw new ArgumentException("Cannot use NpgSql Options. Port was not set.");
        if (string.IsNullOrWhiteSpace(Database))
            throw new ArgumentException("Cannot use NpgSql Options. Database was not set.");
        if (string.IsNullOrWhiteSpace(Username))
            throw new ArgumentException("Cannot use NpgSql Options. Username was not set.");
        if (string.IsNullOrWhiteSpace(Password))
            throw new ArgumentException("Cannot use NpgSql Options. Password was not set.");
    }
}