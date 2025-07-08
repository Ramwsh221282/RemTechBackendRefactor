using RemTech.Core.Shared.Primitives;
using RemTech.Json.Library.Deserialization;
using RemTech.Json.Library.Deserialization.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Json.Deserialization;

public sealed class DeserializedParserIdentity
{
    private readonly ParserIdentity _identity;

    public DeserializedParserIdentity(DesJsonSource source)
    {
        NotEmptyGuid id = NotEmptyGuid.New(new DesJsonGuid(source["id"]));
        Name name = new(NotEmptyString.New(new DesJsonString(source["name"])));
        ParsingType type = ParsingType.New(NotEmptyString.New(new DesJsonString(source["type"])));
        ParserServiceDomain domain = new(NotEmptyString.New(new DesJsonString(source["domain"])));
        _identity = new ParserIdentity(id, name, type, domain);
    }

    public static implicit operator ParserIdentity(DeserializedParserIdentity deserialized) =>
        deserialized._identity;
}
