namespace Scrapers.Module.Features.EnableParser.Models;

internal sealed class UnsupportedEnabledParserTypeException : Exception
{
    public UnsupportedEnabledParserTypeException(string type)
        : base($"Тип парсеров: {type} не поддерживается.") { }

    public UnsupportedEnabledParserTypeException(string type, Exception inner)
        : base($"Тип парсеров: {type} не поддерживается.", inner) { }
}
