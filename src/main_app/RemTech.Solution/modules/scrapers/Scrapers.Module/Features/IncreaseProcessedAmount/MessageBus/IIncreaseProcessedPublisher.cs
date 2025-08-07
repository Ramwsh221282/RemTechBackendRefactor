namespace Scrapers.Module.Features.IncreaseProcessedAmount.MessageBus;

public interface IIncreaseProcessedPublisher
{
    Task SendIncreaseProcessed(string parserName, string parserType, string linkName);
}
