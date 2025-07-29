using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue;

public interface IVehiclesSqlQuery
{
    void AcceptFilter(string filter, NpgsqlParameter parameter);
    void AcceptFilter(string filter, IEnumerable<NpgsqlParameter> parameters);
    void AcceptAscending(string orderingField);
    void AcceptDescending(string orderingField);
    IPgCommandSource PrepareCommand(NpgsqlConnection connection);
    void AcceptPagination(int page);
}
