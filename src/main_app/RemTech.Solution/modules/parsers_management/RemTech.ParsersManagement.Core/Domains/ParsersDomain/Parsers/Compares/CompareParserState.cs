using RemTech.ParsersManagement.Core.Common.Primitives.Comparing;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Compares;

public class CompareParserState : ICompare
{
    private readonly bool _compare;

    public CompareParserState(IParser parser, string relatedState)
        : this(parser.WorkState().Read().StringValue(), relatedState) { }

    public CompareParserState(IParser parser, ParserState relatedState)
        : this(parser.WorkState().Read().StringValue(), relatedState) { }

    public CompareParserState(string parserState, string relatedState) =>
        _compare = parserState == relatedState;

    public bool Equality() => _compare;

    public static implicit operator bool(CompareParserState compare) => compare._compare;
}
