using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ParsersControl.Infrastructure.EventTransporters.PermantlyDisabled;

public sealed class PermantlyDisabledParserEventEmitter
{
    private static readonly RabbitMqPublishOptions _options = new()
    {
        Mandatory = true,
        Persistent = true,
    };    

    private const string Exchange = "parsers";
    private readonly RabbitMqProducer _producer;
    private readonly Serilog.ILogger _logger;
    public PermantlyDisabledParserEventEmitter(RabbitMqProducer producer, Serilog.ILogger logger)
    {
        _producer = producer;
        _logger = logger.ForContext<OnPermantlyDisableManyParsersEventTransporter>();
    }

    public async Task Emit(SubscribedParser parser, CancellationToken ct)
    {
        string domain = parser.Identity.DomainName;
        string type = parser.Identity.ServiceType;        
        ParserStopMessage message = new(parser);  
        string queue = message.CreateQueueString();              
        _logger.Information("Emitting stopping parser for: {Domain} {Type}", domain, type);
        await _producer.PublishDirectAsync<ParserStopMessage>(message, Exchange, queue, _options, ct);
        _logger.Information("Emitted stopping parser for: {Domain} {Type}", domain, type);
    }

    private sealed class ParserStopMessage
    {
        private const string _suffix = "stop";
        public string Domain { get; set; }
        public string Type { get; set; }    

        public ParserStopMessage(SubscribedParser parser)
        {
            Domain = parser.Identity.DomainName;
            Type = parser.Identity.ServiceType;
        }        

        public ParserStopMessage(string domain, string type)
        {
            Domain = domain;
            Type = type;
        }        

        public string CreateQueueString()
        {
            return $"{Domain}.{Type}.{_suffix}";
        }
    }
}