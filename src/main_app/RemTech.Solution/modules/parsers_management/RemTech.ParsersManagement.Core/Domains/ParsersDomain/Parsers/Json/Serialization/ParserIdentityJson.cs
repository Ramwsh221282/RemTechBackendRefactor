using RemTech.Json.Library.Serialization.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Json.Serialization;

public sealed class ParserIdentityJson
{
    private readonly PlainPrimitiveArrayJson _array;

    public ParserIdentityJson(IParser parser)
    {
        ParserIdentity identity = parser.Identification();
        _array = new PlainPrimitiveArrayJson()
            .With(new GuidSerJson("id", identity.ReadId()))
            .With(new StringSerJson("name", identity.ReadName()))
            .With(new StringSerJson("type", identity.ReadType().Read()));
    }

    public static implicit operator PlainPrimitiveArrayJson(ParserIdentityJson json) => json._array;
}
