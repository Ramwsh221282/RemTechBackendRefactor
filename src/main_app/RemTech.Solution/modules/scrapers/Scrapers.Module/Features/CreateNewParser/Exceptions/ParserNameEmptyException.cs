namespace Scrapers.Module.Features.CreateNewParser.Exceptions;

internal sealed class ParserNameEmptyException : Exception
{
    public ParserNameEmptyException()
        : base("Название парсера было пустым.") { }

    public ParserNameEmptyException(Exception inner)
        : base("Название парсера было пустым", inner) { }
}
