namespace Scrapers.Module.Features.RemovingParserLink.Exceptions;

internal sealed class UnableToRemoveParserLinkWhenWorkingException : Exception
{
    public UnableToRemoveParserLinkWhenWorkingException()
        : base("Не удается удалить ссылку у работающего парсера.") { }

    public UnableToRemoveParserLinkWhenWorkingException(Exception inner)
        : base("Не удается удалить ссылку у работающего парсера.", inner) { }
}
