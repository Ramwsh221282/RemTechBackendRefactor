namespace Parsing.RabbitMq.CreateParser;

public sealed record CreateNewParserMessage(string Name, string Type, string Domain);
