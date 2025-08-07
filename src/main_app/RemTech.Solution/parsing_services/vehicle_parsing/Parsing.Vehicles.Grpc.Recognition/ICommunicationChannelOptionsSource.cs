namespace Parsing.Vehicles.Grpc.Recognition;

public interface ICommunicationChannelOptionsSource
{
    ICommunicationChannelOptions Provide();
}
