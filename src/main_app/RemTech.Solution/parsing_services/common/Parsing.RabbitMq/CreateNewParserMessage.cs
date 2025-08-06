namespace Parsing.RabbitMq;

public sealed record CreateNewParserMessage(string Name, string Type, string Domain);
