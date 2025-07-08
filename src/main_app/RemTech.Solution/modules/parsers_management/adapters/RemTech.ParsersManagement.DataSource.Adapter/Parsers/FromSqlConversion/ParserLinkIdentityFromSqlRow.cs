using System.Data.Common;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers.FromSqlConversion;

public sealed class ParserLinkIdentityFromSqlRow(DbDataReader reader)
{
    public ParserLinkIdentity Read() =>
        new(
            new ParserIdentityFromSqlRow(reader).Read(),
            NotEmptyGuid.New(reader.GetGuid(reader.GetOrdinal("link_id"))),
            new Name(NotEmptyString.New(reader.GetString(reader.GetOrdinal("link_name"))))
        );
}
