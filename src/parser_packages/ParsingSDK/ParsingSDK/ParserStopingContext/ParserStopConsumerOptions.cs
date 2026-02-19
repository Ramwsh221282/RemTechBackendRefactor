namespace ParsingSDK.ParserStopingContext;

public sealed class ParserStopConsumerOptions
{
    public required string Domain { get; set; } = string.Empty;
    public required string Type { get; set; } = string.Empty;    

    public (string queue, string routingKey) CreateQueueRoutingKeyPair()
    {
        ValidateOptions();
        string queue = $"{Domain}.{Type}.stop";
        string routingKey = $"{Domain}.{Type}.stop";
        return (queue, routingKey);
    }

    private void ValidateOptions()
    {
        if (string.IsNullOrWhiteSpace(Domain))
        {
            throw new InvalidOperationException($"{nameof(ParserStopConsumerOptions)} {nameof(Domain)} was not set.");
        }

        if (string.IsNullOrWhiteSpace(Type))
        {
            throw new InvalidOperationException($"{nameof(ParserStopConsumerOptions)} {nameof(Domain)}");
        }        
    }
}
