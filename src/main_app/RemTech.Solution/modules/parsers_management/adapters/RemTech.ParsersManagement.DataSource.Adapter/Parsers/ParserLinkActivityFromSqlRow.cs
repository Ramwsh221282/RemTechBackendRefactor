using System.Data.Common;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers;

public sealed class ParserLinkActivityFromSqlRow(DbDataReader reader)
{
    public ParserLinkActivity Read() => new(reader.GetBoolean(reader.GetOrdinal("activity")));
}