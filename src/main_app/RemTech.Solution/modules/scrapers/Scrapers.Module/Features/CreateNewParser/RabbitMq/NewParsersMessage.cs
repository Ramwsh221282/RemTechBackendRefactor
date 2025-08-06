namespace Scrapers.Module.Features.CreateNewParser.RabbitMq;

internal sealed record NewParsersMessage(string Name, string Type, string Domain);
