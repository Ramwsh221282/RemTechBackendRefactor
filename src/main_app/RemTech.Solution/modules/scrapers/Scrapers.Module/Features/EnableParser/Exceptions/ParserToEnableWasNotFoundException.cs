namespace Scrapers.Module.Features.EnableParser.Models;

internal sealed class ParserToEnableWasNotFoundException : Exception
{
    public ParserToEnableWasNotFoundException(EnabledParser parser)
        : base($"Парсер {parser.Name} {parser.State} не найден.") { }

    public ParserToEnableWasNotFoundException(EnabledParser parser, Exception inner)
        : base($"Парсер {parser.Name} {parser.State} не найден.", inner) { }
}
