using System.Reflection;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.Postgres;

namespace Brands.Module.Injection;

public sealed class BrandsModuleStorageUpper : IStorageUpper
{
    private readonly IOptions<DatabaseOptions> _options;

    public BrandsModuleStorageUpper(IOptions<DatabaseOptions> options)
    {
        _options = options;
    }

    public Task UpStorage()
    {
        string connectionString = _options.Value.ToConnectionString();
        Assembly assembly = typeof(BrandsModuleStorageUpper).Assembly;
        DbUpDatabaseUp.UpDatabase(connectionString, assembly);
        return Task.CompletedTask;
    }
}