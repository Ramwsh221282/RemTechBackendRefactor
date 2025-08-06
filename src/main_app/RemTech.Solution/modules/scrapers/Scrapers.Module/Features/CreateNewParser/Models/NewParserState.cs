namespace Scrapers.Module.Features.CreateNewParser.Models;

internal sealed record NewParserState
{
    public string State { get; }

    private NewParserState(string state) => State = state;

    public static NewParserState Create()
    {
        string state = "Отключен";
        return new NewParserState(state);
    }
}
