namespace Scrapers.Module.Features.UpdateWaitDays.Exceptions;

internal sealed class ParserToUpdateWaitDaysNotFoundException : Exception
{
    public ParserToUpdateWaitDaysNotFoundException(string name, string type)
        : base($"Не найден парсер {name} {type}") { }

    public ParserToUpdateWaitDaysNotFoundException(string name, string type, Exception inner)
        : base($"Не найден парсер {name} {type}", inner) { }
}
