using System.Threading.Channels;
using Mailing.Moduled.Cache;
using Mailing.Moduled.Contracts;
using Mailing.Moduled.Models;
using Microsoft.Extensions.Hosting;

namespace Mailing.Moduled.Bus;

internal sealed class MailingBusReceiver(
    MailingSendersCache cache,
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
        logger.Information("{Entrance} started.", nameof(MailingBusReceiver));
        while (!stoppingToken.IsCancellationRequested)
        {
            await _reader.WaitToReadAsync(stoppingToken);
            while (_reader.TryRead(out MailingBusMessage? message))
            {
                CachedMailingSender[] senders = await cache.GetAll();
                if (senders.Length == 0)
                {
                    logger.Warning(
                        "{Entrance} unable to send mailing message. No senders active.",
                        nameof(MailingBusReceiver)
                    );
                    continue;
                }

                CachedMailingSender sender = senders[0];
                IEmailSender behaviourSender = new EmailSender(sender.Email, sender.Key);
                await behaviourSender
                    .FormEmailMessage()
                    .Send(message.To, message.Subject, message.Body, stoppingToken);
                logger.Information("{Entrance} has sended email message. To: {To}.", message.To);
            }
        }
    }
}