namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects;

public sealed class ParserLinkStatisticIncreasement
{
    private readonly Guid _linkId;
    private readonly ParserLinkStatistic _linkStatistic;

    public ParserLinkStatisticIncreasement(IParserLink link)
    {
        _linkId = link.Identification().ReadId();
        _linkStatistic = link.WorkedStatistic();
    }

    public Guid IdOfIncreased() => _linkId;

    public int CurrentProcessed() => _linkStatistic.ProcessedAmount().Read();
}
