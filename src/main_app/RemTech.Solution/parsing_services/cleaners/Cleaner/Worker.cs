using RabbitMQ.Client;
using StackExchange.Redis;

namespace Cleaner;

public sealed class Worker : BackgroundService
{
    private readonly IChannel _channel;
    private readonly IConnection _connection;
    private const string Exchange = "cleaners";
    private const string Queue = "start";
    private readonly ConnectionMultiplexer _multiplexer;
    private readonly Serilog.ILogger _logger;

    public Worker(
        Serilog.ILogger logger,
        ConnectionFactory rabbitMqFactory,
        ConnectionMultiplexer multiplexer
    )
    {
        _logger = logger;
        _multiplexer = multiplexer;
        _connection = rabbitMqFactory.CreateConnectionAsync().Result;
        _channel = _connection.CreateChannelAsync().Result;
        _channel.ExchangeDeclareAsync(Exchange, ExchangeType.Direct, false, false).Wait();
        _channel.QueueDeclareAsync(Queue, false, false, false).Wait();
        _channel.QueueBindAsync(Queue, Exchange, Queue).Wait();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) { }

    // private static async Task(object sender, )
}
