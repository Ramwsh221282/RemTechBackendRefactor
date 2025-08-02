namespace Mailing.Module.Configuration;

internal sealed record MailingModuleDatabaseOptions(
    string Host,
    string Port,
    string Database,
    string UserName,
    string Password
)
{
    private const string ConnectionStringTemplate =
        "Host={0};Port={1};Database={2};Username={3};Password={4};";

    public string ToConnectionString()
    {
        return string.Format(ConnectionStringTemplate, Host, Port, Database, UserName, Password);
    }
}
