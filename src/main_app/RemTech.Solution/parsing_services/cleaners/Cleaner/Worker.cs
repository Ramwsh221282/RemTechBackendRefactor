using Cleaner.Cleaning;
using Cleaner.Constants;
using Parsing.SDK.Browsers;
using PuppeteerSharp;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;

namespace Cleaner;

internal sealed class Worker(
    Serilog.ILogger logger,
    ConnectionFactory factory,
    ConnectionMultiplexer multiplexer
) : BackgroundService
{
    private const string Exchange = RabbitMqConstants.CleanersExchange;
    private const string Queue = RabbitMqConstants.CleanersStartQueue;
    private IChannel? _channel;
    private IConnection? _connection;
    private AsyncEventingBasicConsumer? _consumer;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.Information("Initializing listener for cleaner service.");
        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await _channel.ExchangeDeclareAsync(
            Exchange,
            ExchangeType.Direct,
            cancellationToken: cancellationToken
        );
        await _channel.QueueDeclareAsync(
            Queue,
            false,
            false,
            false,
            cancellationToken: cancellationToken
        );
        await _channel.QueueBindAsync(Queue, Exchange, Queue, cancellationToken: cancellationToken);
        AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += Handle;
        _consumer = consumer;
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_connection == null)
            throw new InvalidOperationException("Connection not initialized");
        if (_channel == null)
            throw new InvalidOperationException("Channel not initialized");
        if (_consumer == null)
            throw new InvalidOperationException("Cleaner service consumer is not initialized.");
        logger.Information("Cleaner service is ready for cleaning.");
        await _channel.BasicConsumeAsync(Queue, false, _consumer, stoppingToken);
        stoppingToken.ThrowIfCancellationRequested();
    }

    private async Task Handle(object sender, BasicDeliverEventArgs @event)
    {
        if (_connection == null)
            throw new InvalidOperationException("Connection not initialized");
        if (_channel == null)
            throw new InvalidOperationException("Channel not initialized");
        await _channel.BasicAckAsync(@event.DeliveryTag, false);
        logger.Information("Cleaner invoked.");
        await using IScrapingBrowser browser = await BrowserFactory.ProvideBrowser();
        await using IPage page = await browser.ProvideDefaultPage();
        try
        {
            SuspiciousItemCollection items = new(@event);
            SuspiciousItemInspector inspector = new(logger, page, _connection, multiplexer);
            foreach (SuspiciousItem item in items.Read())
            {
                await inspector.Inspect(item);
                if (!await inspector.WasPermantlyDisabled())
                    continue;
                await inspector.FinishJob();
                break;
            }
            await inspector.FinishJob();
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", "Cleaner", ex.Message);
        }
    }
}
