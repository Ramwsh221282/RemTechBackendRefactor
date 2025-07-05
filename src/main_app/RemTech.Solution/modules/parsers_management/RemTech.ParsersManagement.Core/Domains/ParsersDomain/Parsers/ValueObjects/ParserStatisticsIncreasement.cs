using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

public sealed class ParserStatisticsIncreasement
{
    private readonly Guid _parserId;
    private readonly ParserStatistic _parserStatistic;
    private readonly ParserLinkStatisticIncreasement _linkIncreasement;

    public ParserStatisticsIncreasement(IParser parser, IParserLink link)
    {
        _linkIncreasement = new ParserLinkStatisticIncreasement(link);
        _parserId = parser.Identification().ReadId();
        _parserStatistic = parser.WorkedStatistics();
        _parserStatistic++;
    }

    public Guid IdOfIncreased() => _parserId;

    public int CurrentProcessed() => _parserStatistic.ProcessedAmount().Read();

    public ParserLinkStatisticIncreasement LinkIncreasement() => _linkIncreasement;
}
