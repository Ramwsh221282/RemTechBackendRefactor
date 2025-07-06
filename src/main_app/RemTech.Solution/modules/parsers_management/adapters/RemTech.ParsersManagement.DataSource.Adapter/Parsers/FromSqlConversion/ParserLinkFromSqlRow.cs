using System.Data.Common;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers.FromSqlConversion;

public sealed class ParserLinkFromSqlRow(DbDataReader reader)
{
    private readonly ParserLinkIdentityFromSqlRow _identity = new(reader);
    private readonly ParserLinkUrlFromSqlRow _url = new(reader);
    private readonly ParserLinkActivityFromSqlRow _activity = new(reader);
    private readonly ParserLinkStatisticFromSqlRow _statistic = new(reader);

    public IParserLink Read() =>
        new ParserLink(_identity.Read(), _url.Read(), _statistic.Read(), _activity.Read());
}
