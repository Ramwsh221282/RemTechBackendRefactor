using System.Threading.Channels;

namespace Mailing.Module.Bus;

public sealed class MailingBusPublisher(Channel<MailingBusMessage> channel)
{
    private readonly ChannelWriter<MailingBusMessage> _writer = channel.Writer;

    public async Task Send(MailingBusMessage message, CancellationToken ct = default) =>
        await _writer.WriteAsync(message, ct);
}
