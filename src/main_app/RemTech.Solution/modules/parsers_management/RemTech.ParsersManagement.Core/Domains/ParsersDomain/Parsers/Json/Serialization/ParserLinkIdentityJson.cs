using RemTech.Json.Library.Serialization.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Json.Serialization;

public sealed class ParserLinkIdentityJson
{
    private readonly PlainPrimitiveArrayJson _array;

    public ParserLinkIdentityJson(IParserLink link)
    {
        ParserLinkIdentity identity = link.Identification();
        _array = new PlainPrimitiveArrayJson()
            .With(new GuidSerJson("id", identity.ReadId()))
            .With(new GuidSerJson("parser_id", identity.OwnerIdentification().ReadId()))
            .With(new StringSerJson("name", identity.ReadName()));
    }

    public static implicit operator PlainPrimitiveArrayJson(ParserLinkIdentityJson json) =>
        json._array;
}
