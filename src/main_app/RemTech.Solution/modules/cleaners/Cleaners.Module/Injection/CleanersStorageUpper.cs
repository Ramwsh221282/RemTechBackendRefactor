using System.Reflection;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.Postgres;

namespace Cleaners.Module.Injection;

public sealed class CleanersStorageUpper : IStorageUpper
{
    private readonly IOptions<DatabaseOptions> _options;

    public CleanersStorageUpper(IOptions<DatabaseOptions> options) => _options = options;

    public Task UpStorage()
    {
        string connectionString = _options.Value.ToConnectionString();
        Assembly assembly = typeof(CleanersStorageUpper).Assembly;
        DbUpDatabaseUp.UpDatabase(connectionString, assembly);
        return Task.CompletedTask;
    }
}