using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Parsing.Vehicles.Grpc.Recognition;

public sealed class JsonCommunicationChannelOptions : ICommunicationChannelOptions
{
    private readonly string _filePath;

    public JsonCommunicationChannelOptions(string filePath)
    {
        _filePath = filePath;
    }

    public void Register(IServiceCollection services)
    {
        IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile(_filePath).Build();
        string? host = root.GetSection("VEHICLES_GRPC_RECOGNITION_ADDRESS").Value;
        if (string.IsNullOrWhiteSpace(host))
            throw new ApplicationException("Vehicles grpc recognition address not configured.");
        services.AddSingleton<ICommunicationChannel>(
            new CommunicationChannel(new CommunicationChannelOptions(host))
        );
    }
}
