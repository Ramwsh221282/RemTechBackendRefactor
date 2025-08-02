using System.Threading.Channels;
using Mailing.Module.Contracts;
using Microsoft.Extensions.Hosting;

namespace Mailing.Module.Bus;

internal sealed class MailingBusReceiver(
    IEmailSendersSource senders,
    Channel<MailingBusMessage> channel
) : BackgroundService
{
    private readonly ChannelReader<MailingBusMessage> _reader = channel.Reader;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_reader.TryRead(out MailingBusMessage? message))
                return;
            IEnumerable<IEmailSender> senders1 = await senders.ReadAll(stoppingToken);
            IEmailSender[] array = senders1.ToArray();
            if (array.Length == 0)
                return;
            IEmailSender first = array[0];
        }
    }
}
