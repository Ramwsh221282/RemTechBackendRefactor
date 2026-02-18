using System.Threading.Channels;

namespace ParsingSDK.ParserStopingContext;

public sealed class ParserStopState
{
    private const int MAX_CONCURRENT_CALLS = 1;
    private static readonly BoundedChannelOptions _options = new(MAX_CONCURRENT_CALLS)
    {
        SingleReader = true,
        SingleWriter = true,
        FullMode = BoundedChannelFullMode.Wait
    };

    private readonly Channel<ParserStopSignal> _channel;    

    public ParserStopState()
    {
        _channel = Channel.CreateBounded<ParserStopSignal>(_options);        
    }

    public async Task RequestStop(CancellationToken ct = default)
    {
        await _channel.Writer.WriteAsync(new ParserStopSignal(), ct);
    }

    public bool HasStopBeenRequested()
    {
        return _channel.Reader.Count > 0;        
    }

    public void CommitStop()
    {
        _channel.Reader.TryRead(out _);
    }
}
