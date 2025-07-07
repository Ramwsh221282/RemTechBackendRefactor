using RemTech.Json.Library.Serialization.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Json.Serialization;

public sealed class ParserWorkScheduleJson
{
    private readonly PlainPrimitiveArrayJson _array;

    public ParserWorkScheduleJson(IParser parser)
    {
        ParserSchedule schedule = parser.WorkSchedule();
        _array = new PlainPrimitiveArrayJson()
            .With(new IntegerSerJson("wait_days", schedule.WaitDays().Read()))
            .With(new DateTimeSerJson("next_run", schedule.NextRun()))
            .With(new DateTimeSerJson("last_run", schedule.LastRun()));
    }

    public static implicit operator PlainPrimitiveArrayJson(ParserWorkScheduleJson json) =>
        json._array;
}
