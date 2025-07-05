using System.Data.Common;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers.FromSqlConversion;

public sealed class ParserScheduleFromSqlRow(DbDataReader reader)
{
    public ParserSchedule Read() =>
        new(
            reader.GetDateTime(reader.GetOrdinal("last_run")),
            reader.GetDateTime(reader.GetOrdinal("next_run")),
            PositiveInteger.New(reader.GetInt32(reader.GetOrdinal("wait_days")))
        );
}
