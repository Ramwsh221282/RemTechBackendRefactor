using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Parsing.Grpc.Services.DuplicateIds;

public sealed class DuplicateIdsCheckClientOptions
{
    private string Address { get; }

    private DuplicateIdsCheckClientOptions(string address)
    {
        Console.WriteLine($"MAIN_GRPC_ADDRESS = {address}");
        Address = address;
    }

    public void Register(IServiceCollection services)
    {
        services.AddSingleton<GrpcDuplicateIdsClient>(_ => new GrpcDuplicateIdsClient(Address));
    }

    public static DuplicateIdsCheckClientOptions Create(bool isDevelopment)
    {
        return isDevelopment ? FromAppSettings("appsettings.json") : FromEnv();
    }

    private static DuplicateIdsCheckClientOptions FromAppSettings(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ApplicationException(
                $"{nameof(DuplicateIdsCheckClientOptions)} file path is empty."
            );
        if (!File.Exists(filePath))
            throw new ApplicationException(
                $"{nameof(DuplicateIdsCheckClientOptions)} file does not exist."
            );
        IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile(filePath).Build();
        string? hostName = root.GetSection("MAIN_GRPC_ADDRESS").Value;
        return string.IsNullOrWhiteSpace(hostName)
            ? throw new ApplicationException(
                "Main Grpc Address. MAIN_GRPC_ADDRESS not found. Appsettings source."
            )
            : new DuplicateIdsCheckClientOptions(hostName);
    }

    private static DuplicateIdsCheckClientOptions FromEnv()
    {
        string? address = Environment.GetEnvironmentVariable("MAIN_GRPC_ADDRESS");
        return string.IsNullOrWhiteSpace(address)
            ? throw new ApplicationException(
                "Main Grpc Address. MAIN_GRPC_ADDRESS not found. Env source."
            )
            : new DuplicateIdsCheckClientOptions(address);
    }
}
