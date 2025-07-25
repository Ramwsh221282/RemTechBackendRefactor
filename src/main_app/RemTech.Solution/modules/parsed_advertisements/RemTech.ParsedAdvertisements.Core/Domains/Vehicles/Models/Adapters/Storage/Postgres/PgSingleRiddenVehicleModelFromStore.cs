using System.Data.Common;
using RemTech.Core.Shared.Exceptions;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Adapters.Storage.Postgres;

public sealed class PgSingleRiddenVehicleModelFromStore
{
    private readonly DbDataReader _reader;

    public PgSingleRiddenVehicleModelFromStore(DbDataReader reader)
    {
        _reader = reader;
    }

    public async Task<VehicleModel> Read()
    {
        if (!await _reader.ReadAsync())
            throw new OperationException("Модель техники не найдена.");
        Guid id = _reader.GetGuid(_reader.GetOrdinal("id"));
        string name = _reader.GetString(_reader.GetOrdinal("text"));
        return new VehicleModel(new VehicleModelIdentity(id), new VehicleModelName(name));
    }
}