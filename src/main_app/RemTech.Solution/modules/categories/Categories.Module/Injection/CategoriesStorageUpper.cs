using System.Reflection;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.Postgres;

namespace Categories.Module.Injection;

public sealed class CategoriesStorageUpper : IStorageUpper
{
    private readonly IOptions<DatabaseOptions> _options;

    public CategoriesStorageUpper(IOptions<DatabaseOptions> options)
    {
        _options = options;
    }

    public Task UpStorage()
    {
        string connectionString = _options.Value.ToConnectionString();
        Assembly assembly = typeof(CategoriesStorageUpper).Assembly;
        DbUpDatabaseUp.UpDatabase(connectionString, assembly);
        return Task.CompletedTask;
    }
}