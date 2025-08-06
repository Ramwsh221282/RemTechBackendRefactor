namespace Scrapers.Module.Features.CreateNewParser.Exceptions;

internal sealed class ParserTypeExceesLengthException : Exception
{
    public ParserTypeExceesLengthException(string type, int allowedLength)
        : base($"Тип парсера {type} превышает длину {allowedLength} символов.") { }

    public ParserTypeExceesLengthException(string type, int allowedLength, Exception inner)
        : base($"Тип парсера {type} превышает длину {allowedLength} символов.") { }
}
