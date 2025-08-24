using System.Data.Common;
using Google.Protobuf.Collections;
using Grpc.Core;
using Npgsql;

namespace RemTech.ContainedItems.Module.Grpc;

public sealed class DuplicateIdsCheckService(NpgsqlDataSource dataSource, Serilog.ILogger logger)
    : DuplicateIdsGrpcService.DuplicateIdsGrpcServiceBase
{
    private const string Context = nameof(DuplicateIdsCheckService);

    private const string Sql = "SELECT id FROM contained_items.items WHERE id = ANY(@ids);";

    public override Task<PingDuplicateIdsServiceResponse> PingService(
        PingDuplicateIdsServiceRequest request,
        ServerCallContext context
    )
    {
        PingDuplicateIdsServiceResponse response = new PingDuplicateIdsServiceResponse();
        return Task.FromResult(response);
    }

    public override async Task<CheckDuplicateIdsResponse> CheckDuplicateIds(
        CheckDuplicateIdsRequest request,
        ServerCallContext context
    )
    {
        try
        {
            RepeatedField<IdsToCheck> identifiers = request.Ids;
            logger.Information(
                "{Context} received request to check duplicate identifiers. Count: {Count}",
                Context,
                identifiers.Count
            );
            if (identifiers.Count == 0)
            {
                logger.Information("Request to check duplicate identifiers contains no items.");
                return new CheckDuplicateIdsResponse();
            }
            string[] stringIdentifiers = identifiers
                .Where(i => !string.IsNullOrWhiteSpace(i.Id))
                .Select(i => i.Id)
                .ToArray();
            CheckDuplicateIdsResponse result = await GetExistingIdentifiers(stringIdentifiers);
            logger.Information("Found duplicate identifiers: {Count}", result.Ids.Count);
            return result;
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", Context, ex.Message);
            return new CheckDuplicateIdsResponse();
        }
    }

    private async Task<CheckDuplicateIdsResponse> GetExistingIdentifiers(string[] identifiers)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = Sql;
        command.Parameters.AddWithValue("@ids", identifiers);
        CheckDuplicateIdsResponse response = new CheckDuplicateIdsResponse();
        await using DbDataReader reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            string id = reader.GetString(0);
            response.Ids.Add(new IdsExisting() { Id = id });
        }
        return response;
    }
}
