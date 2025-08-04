namespace Scrapers.Module.Features.DisablingParser.Exceptions;

internal sealed class UnsupportedDisabledParserTypeException : Exception
{
    public UnsupportedDisabledParserTypeException(string type)
        : base($"Тип парсера {type} не поддерживается.") { }

    public UnsupportedDisabledParserTypeException(string type, Exception inner)
        : base($"Тип парсера {type} не поддерживается.", inner) { }
}
