using Npgsql;
using RemTech.Core.Shared.Exceptions;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Decorators.Postgres;

public sealed class PgVarietVehicleModel(PgConnectionSource connectionSource, VehicleModel origin)
    : VehicleModel(origin)
{
    private readonly Queue<Func<Task<VehicleModel>>> _strategies = [];

    public async Task<VehicleModel> SaveAsync(CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await connectionSource.Connect();
        PgVehicleModel pgModel = new(connection, this);
        _strategies.Enqueue(() => pgModel.SaveAsync(pgModel.ToStoreCommand(), ct));
        _strategies.Enqueue(() => pgModel.SaveAsync(pgModel.FromStoreCommand(), ct));
        while (_strategies.Count > 0)
        {
            try
            {
                Task<VehicleModel> task = _strategies.Dequeue()();
                return await task;
            }
            catch
            {
                // ignored
            }
        }

        throw new OperationException("Невозможно сохранить модель техники.");
    }
}