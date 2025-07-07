using RemTech.Json.Library.Deserialization;
using RemTech.Json.Library.Deserialization.Primitives;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects.WorkingStatistics;
using RemTech.ParsersManagement.Core.Common.ValueObjects.WorkingTimes;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Json.Deserialization;

public sealed class DeserializedParserStatistics
{
    private readonly ParserStatistic _statistic;

    public DeserializedParserStatistics(DesJsonSource source)
    {
        IncrementableNumber processed = new(
            PositiveInteger.New(new DesJsonInteger(source["processed"]))
        );
        PositiveLong total_seconds = PositiveLong.New(new DesJsonLong(source["total_seconds"]));
        Hour hours = new(PositiveInteger.New(new DesJsonInteger(source["hours"])));
        Minutes minutes = new(PositiveInteger.New(new DesJsonInteger(source["minutes"])));
        Seconds seconds = new(PositiveInteger.New(new DesJsonInteger(source["seconds"])));
        WorkingTime time = new(total_seconds, hours, minutes, seconds);
        _statistic = new ParserStatistic(new WorkingStatistic(time, processed));
    }

    public static implicit operator ParserStatistic(DeserializedParserStatistics statistics) =>
        statistics._statistic;
}
