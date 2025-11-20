using RabbitMQ.Client.Events;
using RemTech.Functional.Extensions;
using RemTech.RabbitMq.Abstractions;
using Tickets.Core;

namespace Tickets.EventListeners;

public sealed class TicketCreatedEventListener : RabbitMqListenerHostService
{
    private const string Queue = "tickets";
    private const string Exchange = "tickets";
    private const string RoutingKey = "tickets.create";
    private const string ExchangeType = "topic";
    private const string MessageName = nameof(TicketCreatedEventListener);
    private readonly Serilog.ILogger _logger;
    private readonly IServiceProvider _sp;


    private static AsyncEventHandler<BasicDeliverEventArgs> Handler(IServiceProvider sp, Serilog.ILogger logger)
    {
        return async (_, @event) =>
        {
            JsonMessageFromRabbitMq message = new(@event.Body);
            TicketRequiredMessage ticketRequired = TicketRequiredMessage.FromRabbitMqJson(message);
            Result<Ticket> result = await ticketRequired.Register(sp);
            object[] logArgs = [MessageName, result.IsSuccess, result.Error.Message];
            
            if (result.IsSuccess)
                logger.Information("{Message} handled {Success}", logArgs);
            if (result.IsFailure)
                logger.Information("{Message} handled {Success}. Error: {Error}", logArgs);
        };
    }

    protected override Func<RabbitMqConnectionSource, CancellationToken, Task<RabbitMqListener>> GetListener()
    {
        return async (source, token) =>
        {
            DeclareQueueArgs args = new(Queue, Exchange, RoutingKey, ExchangeType);
            return await source.CreateListener(Handler(_sp, _logger), args, token);
        };
    }
    
    public TicketCreatedEventListener(
        IServiceProvider sp, 
        RabbitMqConnectionSource connectionSource,
        Serilog.ILogger logger) 
        : base(connectionSource)
    {
        _sp = sp;
        _logger = logger;
    }
}