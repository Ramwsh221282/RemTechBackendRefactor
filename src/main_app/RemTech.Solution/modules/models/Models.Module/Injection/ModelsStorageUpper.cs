using System.Reflection;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.Postgres;

namespace Models.Module.Injection;

public sealed class ModelsStorageUpper : IStorageUpper
{
    private readonly IOptions<DatabaseOptions> _options;

    public ModelsStorageUpper(IOptions<DatabaseOptions> options)
    {
        _options = options;
    }

    public Task UpStorage()
    {
        string connectionString = _options.Value.ToConnectionString();
        Assembly assembly = typeof(ModelsStorageUpper).Assembly;
        DbUpDatabaseUp.UpDatabase(connectionString, assembly);
        return Task.CompletedTask;
    }
}