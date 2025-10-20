using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Telemetry.Adapter.Storage.Storage;

public sealed partial class ActionRecordsStorage
{
    public async Task<long> Count(CancellationToken ct = default)
    {
        const string sql = "SELECT COUNT(*) FROM telemetry_module.records";
        IDbConnection connection = context.Database.GetDbConnection();
        return await connection.ExecuteScalarAsync<long>(sql);
    }
}
