using System.Data.Common;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkUrls;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers.FromSqlConversion;

public sealed class ParserLinkUrlFromSqlRow(DbDataReader reader)
{
    public ParserLinkUrl Read() =>
        new(NotEmptyString.New(reader.GetString(reader.GetOrdinal("url"))));
}
