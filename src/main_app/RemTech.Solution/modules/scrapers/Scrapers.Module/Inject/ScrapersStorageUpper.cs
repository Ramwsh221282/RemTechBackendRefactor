using System.Reflection;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.Postgres;

namespace Scrapers.Module.Inject;

public sealed class ScrapersStorageUpper : IStorageUpper
{
    private readonly IOptions<DatabaseOptions> _options;

    public ScrapersStorageUpper(IOptions<DatabaseOptions> options) => _options = options;

    public Task UpStorage()
    {
        string connectionString = _options.Value.ToConnectionString();
        Assembly assembly = typeof(ScrapersStorageUpper).Assembly;
        DbUpDatabaseUp.UpDatabase(connectionString, assembly);
        return Task.CompletedTask;
    }
}