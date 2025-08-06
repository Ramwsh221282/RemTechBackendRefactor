namespace Scrapers.Module.Features.CreateNewParser.Exceptions;

internal sealed class ParserNameAndTypeDuplicateException : Exception
{
    public ParserNameAndTypeDuplicateException(string name, string type)
        : base($"Парсер {name} {type} уже существует в системе.") { }

    public ParserNameAndTypeDuplicateException(string name, string type, Exception inner)
        : base($"Парсер {name} {type} уже существует в системе.", inner) { }
}
