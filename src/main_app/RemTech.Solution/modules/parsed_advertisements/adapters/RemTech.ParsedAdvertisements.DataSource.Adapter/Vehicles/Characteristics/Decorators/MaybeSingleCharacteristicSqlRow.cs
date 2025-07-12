using System.Data.Common;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Characteristics.Decorators;

public sealed class MaybeSingleCharacteristicSqlRow
{
    private readonly DbDataReader _reader;

    public MaybeSingleCharacteristicSqlRow(DbDataReader reader)
    {
        _reader = reader;
    }

    public async Task<MaybeBag<ICharacteristic>> Read(CancellationToken ct = default)
    {
        if (!await _reader.ReadAsync(ct))
            return new MaybeBag<ICharacteristic>();
        Guid id = _reader.GetGuid(_reader.GetOrdinal("id"));
        string text = _reader.GetString(_reader.GetOrdinal("text"));
        return new ExistingCharacteristic(id, text);
    }
}
