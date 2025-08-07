namespace Parsing.Vehicles.Grpc.Recognition;

public sealed class JsonCommunicationChannelOptionsSource(string filePath)
    : ICommunicationChannelOptionsSource
{
    public ICommunicationChannelOptions Provide()
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException(filePath);
        return !filePath.EndsWith(".json")
            ? throw new InvalidOperationException("File was not json.")
            : new JsonCommunicationChannelOptions(filePath);
    }
}
