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
    private readonly BrowserManagerProvider _provider;
    private readonly ParserStopState _state;

    private IChannel? _channel;
    private AsyncEventingBasicConsumer? _consumer;
    private IChannel Channel => _channel ?? throw new InvalidOperationException($"{nameof(ParserStopConsumer)} channel was not initialized.");

    public ParserStopConsumer(
        Serilog.ILogger logger,
        RabbitMqConnectionSource connectionSource,
        IOptions<ParserStopConsumerOptions> options,
        ParserStopState state,
        BrowserManagerProvider provider
    )
    {
        _logger = logger.ForContext<ParserStopConsumer>();
        _connectionSource = connectionSource;
        _options = options;
        _state = state;
        _provider = provider;
    }

    private AsyncEventHandler<BasicDeliverEventArgs> ConsumeHandle => async (_, ea) =>
    {
        _logger.Information("Received stop parser message");
        try
        {
            ParserStopMessage message = ParserStopMessage.FromDeliverEventArgs(ea);
            await HandleStopMessage();            
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

    private async Task HandleStopMessage(int attempts = 10, CancellationToken ct = default)
    {
        int currentAttempt = 0;

        while (currentAttempt <= attempts)
        {            
            try
            {
                await _state.RequestStop(ct);
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
}
