using Grpc.Net.Client;

namespace Parsing.Grpc.Services.DuplicateIds;

public sealed class GrpcDuplicateIdsClient
{
    private readonly GrpcChannel _channel;
    private readonly DuplicateIdsGrpcService.DuplicateIdsGrpcServiceClient _client;

    public GrpcDuplicateIdsClient(string address)
    {
        _channel = GrpcChannel.ForAddress(address);
        _client = new DuplicateIdsGrpcService.DuplicateIdsGrpcServiceClient(_channel);
    }

    public async Task<IEnumerable<string>> GetDuplicateIdentifiers(IEnumerable<string> idsToCheck)
    {
        CheckDuplicateIdsRequest request = new CheckDuplicateIdsRequest();
        request.Ids.AddRange(idsToCheck.Select(i => new IdsToCheck() { Id = i }));
        CheckDuplicateIdsResponse response = await _client.CheckDuplicateIdsAsync(request);
        return response.Ids.Select(i => i.Id);
    }

    public async Task<PingDuplicateIdsServiceResponse> Ping()
    {
        PingDuplicateIdsServiceRequest request = new();
        return await _client.PingServiceAsync(request);
    }
}
