namespace Scrapers.Module.Features.CreateNewParser.Exceptions;

internal sealed class ParserTypeEmptyException : Exception
{
    public ParserTypeEmptyException()
        : base("Тип парсера был пустым.") { }

    public ParserTypeEmptyException(Exception inner)
        : base("Тип парсера был пустым.", inner) { }
}
