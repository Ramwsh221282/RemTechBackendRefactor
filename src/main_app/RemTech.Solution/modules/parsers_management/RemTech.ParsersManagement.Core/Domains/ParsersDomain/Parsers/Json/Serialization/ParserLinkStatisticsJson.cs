using RemTech.Json.Library.Serialization.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Json.Serialization;

public sealed class ParserLinkStatisticsJson
{
    private readonly PlainPrimitiveArrayJson _array;

    public ParserLinkStatisticsJson(IParserLink link)
    {
        ParserLinkStatistic statistics = link.WorkedStatistic();
        _array = new PlainPrimitiveArrayJson()
            .With(new IntegerSerJson("processed", statistics.ProcessedAmount()))
            .With(new LongSerJson("total_seconds", statistics.WorkedTime().Total()))
            .With(new IntegerSerJson("hours", statistics.WorkedTime().Hours()))
            .With(new IntegerSerJson("minutes", statistics.WorkedTime().Minutes()))
            .With(new IntegerSerJson("seconds", statistics.WorkedTime().Seconds()));
    }

    public static implicit operator PlainPrimitiveArrayJson(ParserLinkStatisticsJson json) =>
        json._array;
}
