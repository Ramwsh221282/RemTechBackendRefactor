using System.Data.Common;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkActivities;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers.FromSqlConversion;

public sealed class ParserLinkActivityFromSqlRow(DbDataReader reader)
{
    public ParserLinkActivity Read() => new(reader.GetBoolean(reader.GetOrdinal("activity")));
}
