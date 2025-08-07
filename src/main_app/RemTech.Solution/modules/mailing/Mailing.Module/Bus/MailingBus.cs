using System.Threading.Channels;
using Mailing.Module.Contracts;
using Microsoft.Extensions.Hosting;

namespace Mailing.Module.Bus;

internal sealed class MailingBusReceiver(
    IEmailSendersSource senders,
    Channel<MailingBusMessage> channel,
    Serilog.ILogger logger
) : BackgroundService
{
    private readonly ChannelReader<MailingBusMessage> _reader = channel.Reader;

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.Information("{Entrance} starting.", nameof(MailingBusReceiver));
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _reader.WaitToReadAsync(stoppingToken);
            while (_reader.TryRead(out var message))
            {
                IEnumerable<IEmailSender> existingSenders = await senders.ReadAll(stoppingToken);
                IEmailSender[] array = existingSenders.ToArray();
                if (array.Length == 0)
                    return;
                IEmailSender firstSender = array[0];
            }
        }
    }
}
