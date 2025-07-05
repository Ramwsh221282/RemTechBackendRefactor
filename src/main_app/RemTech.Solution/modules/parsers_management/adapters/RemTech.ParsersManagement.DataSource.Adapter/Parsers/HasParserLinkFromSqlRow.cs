using System.Data.Common;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers;

public sealed class HasParserLinkFromSqlRow(DbDataReader reader)
{
    public async Task<bool> Read() => !await reader.IsDBNullAsync(reader.GetOrdinal("link_id"));
}
