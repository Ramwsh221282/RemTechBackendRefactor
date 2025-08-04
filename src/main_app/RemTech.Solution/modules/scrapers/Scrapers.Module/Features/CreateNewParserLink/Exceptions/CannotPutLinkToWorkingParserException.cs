namespace Scrapers.Module.Features.CreateNewParserLink.Exceptions;

internal sealed class CannotPutLinkToWorkingParserException : Exception
{
    public CannotPutLinkToWorkingParserException()
        : base("Нельзя добавить ссылку в работающий парсер.") { }

    public CannotPutLinkToWorkingParserException(Exception inner)
        : base("Нельзя добавить ссылку в работающий парсер.", inner) { }
}
