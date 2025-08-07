using Microsoft.Extensions.DependencyInjection;

namespace Parsing.Vehicles.Grpc.Recognition;

public interface ICommunicationChannelOptions
{
    void Register(IServiceCollection services);
}
