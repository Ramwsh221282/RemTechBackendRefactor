namespace Scrapers.Module.Features.CreateNewParserLink.Exceptions;

internal sealed class UnkownParserStateException : Exception
{
    public UnkownParserStateException()
        : base("Не удается узнать состояние парсера.") { }
}
