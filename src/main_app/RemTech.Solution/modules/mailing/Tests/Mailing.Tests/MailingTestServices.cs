using Testcontainers.PostgreSql;

namespace Mailing.Tests;

public sealed class MailingTestServices
{
    private readonly PostgreSqlContainer _container;

    private readonly Lazy<IServiceProvider> _services;
}