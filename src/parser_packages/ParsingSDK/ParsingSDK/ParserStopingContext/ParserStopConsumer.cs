using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ParsingSDK.Parsing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ParsingSDK.ParserStopingContext;

public sealed class ParserStopConsumer : IConsumer
{    
    private readonly Serilog.ILogger _logger;
    private readonly RabbitMqConnectionSource _connectionSource;
    private readonly IOptions<ParserStopConsumerOptions> _options;
    private readonly IServiceProvider _services;
    private readonly BrowserManagerProvider _provider;
    private IChannel? _channel;
    private AsyncEventingBasicConsumer? _consumer;
    private IChannel Channel => _channel ?? throw new InvalidOperationException($"{nameof(ParserStopConsumer)} channel was not initialized.");

    public ParserStopConsumer(
        Serilog.ILogger logger,
        RabbitMqConnectionSource connectionSource,
        IOptions<ParserStopConsumerOptions> options,
        IServiceProvider services,
        BrowserManagerProvider provider
    )
    {
        _logger = logger.ForContext<ParserStopConsumer>();
        _connectionSource = connectionSource;
        _options = options;
        _services = services;
        _provider = provider;
    }

    private AsyncEventHandler<BasicDeliverEventArgs> ConsumeHandle => async (_, ea) =>
    {
        _logger.Information("Received stop parser message");
        try
        {
            ParserStopMessage message = ParserStopMessage.FromDeliverEventArgs(ea);
            await HandleStopMessage(message);
            await DestroyBrowser();
        }
        catch(Exception ex)
        {
            _logger.Fatal(ex, "Error at handling: {message}", nameof(ParserStopMessage));
        }
        finally
        {
            await Channel.BasicAckAsync(ea.DeliveryTag, false);
        }
    };

    public async Task InitializeChannel(IConnection connection, CancellationToken ct = default)
    {
        EnsureParserStopInterfaceImplementationIsRegistered(_services);
        ParserStopConsumerOptions value = _options.Value;        
        (string queue, string routingKey) = value.CreateQueueRoutingKeyPair();
        _channel = await TopicConsumerInitialization.InitializeChannel(_connectionSource, "parsers", queue, routingKey, ct);
    }

    public Task Shutdown(CancellationToken ct = default)
    {
        return Channel.CloseAsync(cancellationToken: ct);        
    }

    public async Task StartConsuming(CancellationToken ct = default)
    {
        _consumer = new AsyncEventingBasicConsumer(Channel);
        _consumer.ReceivedAsync += ConsumeHandle;
        (string queue, _) = _options.Value.CreateQueueRoutingKeyPair();
        await Channel.BasicConsumeAsync(queue, false, _consumer, cancellationToken: ct);
    }    

    private async Task HandleStopMessage(ParserStopMessage message, int attempts = 10, CancellationToken ct = default)
    {
        int currentAttempt = 0;

        while (currentAttempt <= attempts)
        {            
            try
            {
                await using AsyncServiceScope scope = _services.CreateAsyncScope();
                IParserStopper stopper = scope.ServiceProvider.GetRequiredService<IParserStopper>();
                await stopper.Stop(message.Domain, message.Type, ct);
                break;
            }
            catch(Exception ex)
            {
                currentAttempt++;
                _logger.Error(ex, "Error at stopping parser work. Attempt: {attempt}.", currentAttempt);                
                await Task.Delay(TimeSpan.FromSeconds(5), ct);
            }
        }        
    }

    private static void EnsureParserStopInterfaceImplementationIsRegistered(IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();
        IParserStopper? stopper = scope.ServiceProvider.GetService<IParserStopper>();
        if (stopper is null)
        {
            throw new InvalidOperationException($"{nameof(ParserStopConsumer)} implementation of {nameof(IParserStopper)} is not registered.");
        }
    }

    private async Task DestroyBrowser()
    {
        BrowserManager browser = _provider.Provide();
        await browser.DisposeAsync();
        browser.ReleasePageUsedMemoryResources();
    }
}
