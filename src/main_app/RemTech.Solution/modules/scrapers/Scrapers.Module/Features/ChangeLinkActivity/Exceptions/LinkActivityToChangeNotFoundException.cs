namespace Scrapers.Module.Features.ChangeLinkActivity.Exceptions;

internal sealed class LinkActivityToChangeNotFoundException : Exception
{
    public LinkActivityToChangeNotFoundException(string name, string parserName, string parserType)
        : base($"Не удается найти ссылку {name} {parserName} {parserType}") { }

    public LinkActivityToChangeNotFoundException(
        string name,
        string parserName,
        string parserType,
        Exception inner
    )
        : base($"Не удается найти ссылку {name} {parserName} {parserType}", inner) { }
}
