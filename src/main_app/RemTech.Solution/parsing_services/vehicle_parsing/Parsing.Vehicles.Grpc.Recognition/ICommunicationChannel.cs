namespace Parsing.Vehicles.Grpc.Recognition;

public interface ICommunicationChannel : IDisposable
{
    Talker Talker();
}
