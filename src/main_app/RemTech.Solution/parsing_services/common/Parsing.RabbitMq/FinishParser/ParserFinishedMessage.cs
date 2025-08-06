namespace Parsing.RabbitMq.FinishParser;

public record ParserFinishedMessage(string ParserName, string ParserType, long TotalElapsedSeconds);
