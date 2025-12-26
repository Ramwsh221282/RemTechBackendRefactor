using System.Reflection;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.Postgres;

namespace GeoLocations.Module.Injection;

public sealed class GeoLocationsStorageUpper : IStorageUpper
{
    private readonly IOptions<DatabaseOptions> _options;

    public GeoLocationsStorageUpper(IOptions<DatabaseOptions> options)
    {
        _options = options;
    }

    public Task UpStorage()
    {
        string connectionString = _options.Value.ToConnectionString();
        Assembly assembly = typeof(GeoLocationsStorageUpper).Assembly;
        DbUpDatabaseUp.UpDatabase(connectionString, assembly);
        return Task.CompletedTask;
    }
}