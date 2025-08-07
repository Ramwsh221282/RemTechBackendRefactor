namespace Scrapers.Module.Features.InstantlyEnableParser.Models;

internal sealed class UnableToInstantlyStartParserWithoutLinksException : Exception
{
    public UnableToInstantlyStartParserWithoutLinksException()
        : base("Нельзя немедленно запустить парсер без ссылок.") { }

    public UnableToInstantlyStartParserWithoutLinksException(Exception ex)
        : base("Нельзя немедленно запустить парсер без ссылок.", ex) { }
}
