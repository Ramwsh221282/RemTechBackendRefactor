using System.Data;
using Dapper;
using Pgvector;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using Vehicles.Domain.Locations;
using Vehicles.Domain.Locations.Contracts;

namespace Vehicles.Infrastructure.Locations.LocationsPersisterImplementation;

public sealed class NpgSqlLocationsPersister(NpgSqlSession session, EmbeddingsProvider embeddings) : ILocationsPersister
{
    public async Task<Result<Location>> Save(Location location, CancellationToken ct = default)
    {
        Result<NpgSqlSearchResult> fullResult = await TrySearchFullInfo(location.Name.Value, ct);
        if (fullResult.IsFailure) return Error.NotFound("Unable to resolve location.");
        return CreateLocationFromSearchResult(fullResult);
    }

    private async Task<Result<NpgSqlSearchResult>> TrySearchFullInfo(string rawText,
        CancellationToken ct)
    {
        const string sql = """
                           WITH region_search AS (
                            SELECT id as region_id, name as region_name, kind as region_kind, distance
                                                      FROM (
                                                          SELECT r.id, r.name, r.kind, r.embedding <-> v.val AS distance,
                                                                 ROW_NUMBER() OVER (ORDER BY r.embedding <-> v.val) as rank
                                                          FROM UNNEST(@embeddings) AS v(val)
                                                          CROSS JOIN LATERAL (
                                                              SELECT id, name, kind, embedding
                                                              FROM vehicles_module.regions
                                                              WHERE embedding <=> v.val < @region_max_distance
                                                              ORDER BY embedding <=> v.val
                                                              LIMIT 1
                                                          ) r
                                                      ) ranked
                                                      WHERE rank = 1
                           ),
                               city_search AS (
                                   SELECT name as city_name, id as city_id, distance FROM (
                                       SELECT c.name, c.id, c.embedding <-> v.val as distance,
                                              ROW_NUMBER() OVER (ORDER BY c.embedding <-> v.val) as rank
                                       FROM UNNEST(@embeddings) AS v(val)
                                       CROSS JOIN LATERAL (
                                           SELECT name, id, embedding
                                           FROM vehicles_module.cities
                                           WHERE embedding <=> v.val < @city_max_distance
                                           ORDER BY embedding <=> v.val
                                           LIMIT 1
                                           ) c
                                   ) ranked
                                   WHERE rank = 1
                               )
                           SELECT 
                               region_id, 
                               region_name, 
                               region_kind,
                               city_search.city_name as city_name,
                               city_search.city_id as city_id
                           FROM region_search
                           FULL JOIN city_search ON true;
                           """;
        
        string[] parts = rawText.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        Vector[] vectors = parts.Select(part => new Vector(embeddings.Generate(part))).ToArray();

        DynamicParameters parameters = new();
        parameters.Add("@embeddings", vectors);
        parameters.Add("@region_max_distance", 0.2);
        parameters.Add("@city_max_distance", 0.5);

        CommandDefinition command = new(sql, parameters, transaction: session.Transaction, cancellationToken: ct);
        NpgSqlSearchResult? result = await session.QuerySingleUsingReader(command, MapFullFromReader);
        return result is null ? Error.NotFound("Unable to resolve location") : Result.Success(result);
    }

    private static Location CreateLocationFromSearchResult(NpgSqlSearchResult result)
    {
        List<string> locationParts = [];
        Guid locationId = result.Region.RegionId;
        locationParts.Add(result.Region.RegionName);
        locationParts.Add(result.Region.RegionKind);
        if (!string.IsNullOrWhiteSpace(result.City.CityName))
            locationParts.Add(result.City.CityName);
        string fullName = string.Join(", ", locationParts);
        return new Location(LocationId.Create(locationId), LocationName.Create(fullName));
    }

    private static RegionSearchResult MapRegionFromReader(IDataReader reader)
    {
        Guid regionId = reader.GetValue<Guid>("region_id");
        string regionName = reader.GetValue<string>("region_name");
        string regionKind = reader.GetValue<string>("region_kind");
        return new RegionSearchResult
        {
            RegionId = regionId,
            RegionName = regionName,
            RegionKind = regionKind,
        };
    }
    
    private static CitySearchResult MapCityFromReader(IDataReader reader)
    {
        Guid? cityId = reader.GetNullable<Guid>("city_id");
        string? cityName = reader.GetNullableReferenceType<string>("city_name");
        return new CitySearchResult
        {
            CityId = cityId,
            CityName = cityName,
        };
    }

    private static NpgSqlSearchResult MapFullFromReader(IDataReader reader)
    {
        RegionSearchResult region = MapRegionFromReader(reader);
        CitySearchResult city = MapCityFromReader(reader);
        return new NpgSqlSearchResult()
        {
            Region = region,
            City = city
        };
    }
    
    private sealed class RegionSearchResult
    {
        public required Guid RegionId { get; init; }
        public required string RegionName { get; init; }
        public required string RegionKind { get; init; }
    }
    
    public sealed class CitySearchResult
    {
        public required Guid? CityId { get; init; }
        public required string? CityName { get; init; }
    }
    
    private sealed class NpgSqlSearchResult
    {
        public required RegionSearchResult Region { get; init; }
        public required CitySearchResult City { get; init; }
    }
}
    