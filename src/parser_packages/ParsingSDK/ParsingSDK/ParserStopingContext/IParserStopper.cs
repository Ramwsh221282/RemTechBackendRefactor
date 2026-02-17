namespace ParsingSDK.ParserStopingContext;

public interface IParserStopper
{
    Task Stop(string Domain, string Type, CancellationToken ct = default);
}
