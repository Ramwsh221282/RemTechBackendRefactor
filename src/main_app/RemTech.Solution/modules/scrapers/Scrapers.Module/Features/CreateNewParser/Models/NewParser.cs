using Scrapers.Module.Features.CreateNewParser.Exceptions;

namespace Scrapers.Module.Features.CreateNewParser.Models;

internal sealed record NewParser
{
    private const int AllowedNameLength = 20;
    public string Name { get; }
    public NewParserStatistic Statistics { get; }
    public NewParserSchedule Schedule { get; }
    public NewParserState State { get; }
    public NewParserDomain Domain { get; }
    public NewParserType Type { get; }

    private NewParser(
        string name,
        NewParserStatistic statistics,
        NewParserSchedule schedule,
        NewParserState state,
        NewParserDomain domain,
        NewParserType type
    )
    {
        Name = name;
        Statistics = statistics;
        Schedule = schedule;
        State = state;
        Domain = domain;
        Type = type;
    }

    public static NewParser Create(string name, string type, string domain)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ParserNameEmptyException();
        if (name.Length > AllowedNameLength)
            throw new ParserNameExceesLengthException(name, AllowedNameLength);
        NewParserStatistic statistic = NewParserStatistic.Create();
        NewParserSchedule schedule = NewParserSchedule.Create();
        NewParserState state = NewParserState.Create();
        NewParserDomain parserDomain = NewParserDomain.Create(domain);
        NewParserType parserType = NewParserType.Create(type);
        return new NewParser(name, statistic, schedule, state, parserDomain, parserType);
    }
}
