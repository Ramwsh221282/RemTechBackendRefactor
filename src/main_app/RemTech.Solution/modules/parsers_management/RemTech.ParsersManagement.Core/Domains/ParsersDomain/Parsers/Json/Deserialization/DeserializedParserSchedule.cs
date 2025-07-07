using RemTech.Json.Library.Deserialization;
using RemTech.Json.Library.Deserialization.Primitives;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Json.Deserialization;

public sealed class DeserializedParserSchedule
{
    private readonly ParserSchedule _schedule;

    public DeserializedParserSchedule(DesJsonSource source)
    {
        PositiveInteger wait_days = PositiveInteger.New(new DesJsonInteger(source["wait_days"]));
        DateTime nextRun = new DesJsonDateTime(source["next_run"]);
        DateTime lastRun = new DesJsonDateTime(source["last_run"]);
        _schedule = new ParserSchedule(lastRun, nextRun, wait_days);
    }

    public static implicit operator ParserSchedule(DeserializedParserSchedule statistics) =>
        statistics._schedule;
}
