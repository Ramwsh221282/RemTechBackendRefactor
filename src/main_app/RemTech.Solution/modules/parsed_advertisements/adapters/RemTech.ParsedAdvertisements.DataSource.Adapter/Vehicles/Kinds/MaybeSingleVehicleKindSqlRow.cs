using System.Data.Common;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Kinds;

public sealed class MaybeSingleVehicleKindSqlRow(DbDataReader reader)
{
    public async Task<MaybeBag<IVehicleKind>> Read(CancellationToken ct = default) =>
        !await reader.ReadAsync(ct)
            ? new MaybeBag<IVehicleKind>()
            : new ExistingVehicleKind(
                reader.GetGuid(reader.GetOrdinal("id")),
                reader.GetString(reader.GetOrdinal("text"))
            );
}
