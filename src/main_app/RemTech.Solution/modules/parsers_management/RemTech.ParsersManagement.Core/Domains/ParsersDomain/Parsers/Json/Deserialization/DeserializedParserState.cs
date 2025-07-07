using RemTech.Json.Library.Deserialization;
using RemTech.Json.Library.Deserialization.Primitives;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Json.Deserialization;

public sealed class DeserializedParserState
{
    private readonly ParserState _state;

    public DeserializedParserState(DesJsonSource source)
    {
        _state = ParserState.New(NotEmptyString.New(new DesJsonString(source["state"])));
    }

    public static implicit operator ParserState(DeserializedParserState state) => state._state;
}
