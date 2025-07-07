using RemTech.Json.Library.Serialization.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Json.Serialization;

public sealed class ParserWorkStatisticsJson
{
    private readonly PlainPrimitiveArrayJson _array;

    public ParserWorkStatisticsJson(IParser parser)
    {
        ParserStatistic statistics = parser.WorkedStatistics();
        _array = new PlainPrimitiveArrayJson()
            .With(new IntegerSerJson("processed", statistics.ProcessedAmount()))
            .With(new LongSerJson("total_seconds", statistics.WorkedTime().Total()))
            .With(new IntegerSerJson("hours", statistics.WorkedTime().Hours()))
            .With(new IntegerSerJson("minutes", statistics.WorkedTime().Minutes()))
            .With(new IntegerSerJson("seconds", statistics.WorkedTime().Seconds()));
    }

    public static implicit operator PlainPrimitiveArrayJson(ParserWorkStatisticsJson json) =>
        json._array;
}
