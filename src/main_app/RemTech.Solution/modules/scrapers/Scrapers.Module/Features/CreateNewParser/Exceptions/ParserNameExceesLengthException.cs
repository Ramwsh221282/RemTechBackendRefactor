namespace Scrapers.Module.Features.CreateNewParser.Exceptions;

internal sealed class ParserNameExceesLengthException : Exception
{
    public ParserNameExceesLengthException(string name, int allowedLength)
        : base($"Название парсера {name} превышает длину {allowedLength} символов") { }

    public ParserNameExceesLengthException(string name, int allowedLength, Exception inner)
        : base($"Название парсера {name} превышает длину {allowedLength} символов", inner) { }
}
