using System.Threading.Channels;

namespace Cleaners.Module.Contracts.ItemCleaned;

internal sealed class ItemCleanedMessagePublisher(Channel<ItemCleanedMessage> channel)
{
    private readonly ChannelWriter<ItemCleanedMessage> _writer = channel.Writer;

    public async Task Send(string id, CancellationToken ct = default)
    {
        ItemCleanedMessage message = new(id);
        await Send(message, ct);
    }

    public async Task Send(ItemCleanedMessage message, CancellationToken ct = default) =>
        await _writer.WriteAsync(message, ct);
}
