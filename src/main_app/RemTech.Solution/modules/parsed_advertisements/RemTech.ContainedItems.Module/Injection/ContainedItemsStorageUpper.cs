using System.Reflection;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.Postgres;

namespace RemTech.ContainedItems.Module.Injection;

public sealed class ContainedItemsStorageUpper : IStorageUpper
{
    private readonly IOptions<DatabaseOptions> _options;

    public ContainedItemsStorageUpper(IOptions<DatabaseOptions> options)
    {
        _options = options;
    }

    public Task UpStorage()
    {
        string connectionString = _options.Value.ToConnectionString();
        Assembly assembly = typeof(ContainedItemsStorageUpper).Assembly;
        DbUpDatabaseUp.UpDatabase(connectionString, assembly);
        return Task.CompletedTask;
    }
}