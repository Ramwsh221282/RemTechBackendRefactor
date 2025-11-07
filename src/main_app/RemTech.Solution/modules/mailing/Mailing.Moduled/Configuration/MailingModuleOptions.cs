using Microsoft.Extensions.Configuration;

namespace Mailing.Moduled.Configuration;

public sealed record MailingModuleOptions
{
    internal MailingModuleDatabaseOptions Database { get; }

    public MailingModuleOptions(string configFile)
    {
        Database = ReadFromConfig(configFile);
    }

    private MailingModuleDatabaseOptions ReadFromConfig(string configFile)
    {
        IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile(configFile).Build();
        IConfigurationSection section = root.GetSection(nameof(MailingModuleOptions))
            .GetSection(nameof(MailingModuleDatabaseOptions));
        string? host = section.GetValue<string>("Host");
        string? port = section.GetValue<string>("Port");
        string? database = section.GetValue<string>("Database");
        string? userName = section.GetValue<string>("UserName");
        string? password = section.GetValue<string>("Password");
        if (string.IsNullOrWhiteSpace(host))
            throw new ApplicationException(
                $"{nameof(MailingModuleDatabaseOptions)} empty host database value."
            );
        if (string.IsNullOrWhiteSpace(port))
            throw new ApplicationException(
                $"{nameof(MailingModuleDatabaseOptions)} empty port database value."
            );
        if (string.IsNullOrWhiteSpace(database))
            throw new ApplicationException(
                $"{nameof(MailingModuleDatabaseOptions)} empty database name value."
            );
        if (string.IsNullOrWhiteSpace(userName))
            throw new ApplicationException(
                $"{nameof(MailingModuleDatabaseOptions)} empty database user name value."
            );
        if (string.IsNullOrWhiteSpace(password))
            throw new ApplicationException(
                $"{nameof(MailingModuleDatabaseOptions)} empty password database value."
            );
        return new MailingModuleDatabaseOptions(host, port, database, userName, password);
    }
}