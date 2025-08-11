using System.Threading.Channels;

namespace RemTech.ContainedItems.Module.Features.MessageBus;

internal sealed class AddContainedItemsPublisher(Channel<AddContainedItemMessage> channel)
    : IAddContainedItemsPublisher
{
    private readonly ChannelWriter<AddContainedItemMessage> _writer = channel.Writer;

    public async Task Publish(AddContainedItemMessage message)
    {
        await _writer.WriteAsync(message);
    }
}
