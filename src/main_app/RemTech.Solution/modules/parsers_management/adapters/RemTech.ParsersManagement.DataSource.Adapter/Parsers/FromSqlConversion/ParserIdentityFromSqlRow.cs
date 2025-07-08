using System.Data.Common;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers.FromSqlConversion;

public sealed class ParserIdentityFromSqlRow(DbDataReader reader)
{
    public ParserIdentity Read() =>
        new(
            NotEmptyGuid.New(reader.GetGuid(reader.GetOrdinal("id"))),
            new Name(NotEmptyString.New(reader.GetString(reader.GetOrdinal("name")))),
            ParsingType.New(NotEmptyString.New(reader.GetString(reader.GetOrdinal("type")))),
            new ParserServiceDomain(
                NotEmptyString.New(reader.GetString(reader.GetOrdinal("domain")))
            )
        );
}
