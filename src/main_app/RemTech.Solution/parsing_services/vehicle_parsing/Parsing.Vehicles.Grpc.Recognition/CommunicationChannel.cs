using Grpc.Net.Client;

namespace Parsing.Vehicles.Grpc.Recognition;

public sealed class CommunicationChannel : ICommunicationChannel
{
    private readonly GrpcChannel _channel;
    private readonly Recognizer.Recognizer.RecognizerClient _client;

    public CommunicationChannel(CommunicationChannelOptions options)
    {
        _channel = GrpcChannel.ForAddress(options.Address);
        _client = new Recognizer.Recognizer.RecognizerClient(_channel);
    }

    public void Dispose()
    {
        _channel.Dispose();
    }

    public Talker Talker()
    {
        return new Talker(_client);
    }
}
