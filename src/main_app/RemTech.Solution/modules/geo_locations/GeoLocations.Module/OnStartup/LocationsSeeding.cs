using GeoLocations.Module.Features.Persisting;
using Microsoft.Extensions.Hosting;
using Npgsql;
using RemTech.Vehicles.Module.Database.Embeddings;

namespace GeoLocations.Module.OnStartup;

public sealed class LocationsSeeding(
    NpgsqlDataSource dataSource,
    IEmbeddingGenerator generator,
    Serilog.ILogger logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        long amount = await GetCurrentCount();
        if (amount != 0)
        {
            logger.Information("Regions seeded. Seeding is not required.");
            return;
        }

        CsvLocationsReading reading = new CsvLocationsReading();
        IEnumerable<RegionToPersist> regions = reading.Read();
        foreach (RegionToPersist region in regions)
            await region.Persist(dataSource, generator);
        logger.Information("Regions seeded.");
        await Task.CompletedTask;
    }

    private async Task<long> GetCurrentCount()
    {
        string sql = string.Intern("SELECT COUNT(id) FROM locations_module.regions");
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        object? amount = await command.ExecuteScalarAsync();
        return (long)amount!;
    }
}
