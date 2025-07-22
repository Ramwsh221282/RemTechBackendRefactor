using Grpc.Net.Client;

namespace Parsing.Vehicles.Grpc.Recognition;

public sealed class CommunicationChannel
{
    private readonly GrpcChannel _channel;
    private readonly Recognizer.Recognizer.RecognizerClient _client;

    public CommunicationChannel(string connectionAddress)
    {
        _channel = GrpcChannel.ForAddress(connectionAddress);
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