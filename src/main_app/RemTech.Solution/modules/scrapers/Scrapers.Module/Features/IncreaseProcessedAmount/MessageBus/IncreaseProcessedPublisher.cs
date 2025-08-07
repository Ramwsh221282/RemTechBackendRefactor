using System.Threading.Channels;

namespace Scrapers.Module.Features.IncreaseProcessedAmount.MessageBus;

internal sealed class IncreaseProcessedPublisher(Channel<IncreaseProcessedMessage> channel)
    : IIncreaseProcessedPublisher
{
    private readonly ChannelWriter<IncreaseProcessedMessage> _writer = channel.Writer;

    public async Task SendIncreaseProcessed(string parserName, string parserType, string linkName)
    {
        IncreaseProcessedMessage message = new(parserName, parserType, linkName);
        await _writer.WriteAsync(message);
    }
}
