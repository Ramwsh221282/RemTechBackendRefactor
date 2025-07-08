using RemTech.Json.Library.Deserialization;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Json.Deserialization;

public sealed class DeserializedParser : IDisposable
{
    private readonly DesJsonSource _source;

    public DeserializedParser(DesJsonSource source)
    {
        _source = source;
    }

    public IParser Map()
    {
        ParserIdentity identity = new DeserializedParserIdentity(_source);
        ParserSchedule schedule = new DeserializedParserSchedule(_source);
        ParserState state = new DeserializedParserState(_source);
        ParserStatistic statistic = new DeserializedParserStatistics(_source);
        return new ValidParser(
            new Parser(
                identity,
                statistic,
                schedule,
                state,
                new DeserializedParserLinks(
                    new Parser(identity, statistic, schedule, state),
                    _source
                )
            )
        );
    }

    public void Dispose()
    {
        _source.Dispose();
    }
}
