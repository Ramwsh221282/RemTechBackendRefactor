namespace ParsingSDK.RabbitMq;

public sealed record FinishParserMessage(Guid Id, long TotalElapsedSeconds);