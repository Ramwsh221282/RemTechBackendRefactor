namespace Scrapers.Module.Features.ChangeLinkActivity.Exceptions;

internal sealed class UnableToChangeLinkActivityOfWorkingParserException : Exception
{
    public UnableToChangeLinkActivityOfWorkingParserException()
        : base("Нельзя поменять активность ссылки у работающего парсера.") { }
}
