using System.Data.Common;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers;

public sealed class ParserStateFromSqlRow(DbDataReader reader)
{
    public ParserState Read() =>
        ParserState.New(NotEmptyString.New(reader.GetString(reader.GetOrdinal("state"))));
}