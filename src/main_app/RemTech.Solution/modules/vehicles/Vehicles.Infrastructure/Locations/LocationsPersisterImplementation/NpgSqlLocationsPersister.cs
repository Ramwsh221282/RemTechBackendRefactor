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
        const string sql = """
                           WITH cities_embedding_search AS (
                            SELECT id, name, embedding <-> @input_embedding AS distance
                            FROM vehicles_module.cities
                            WHERE 1 - (embedding <-> @input_embedding) < @max_distance
                            ORDER BY distance
                            LIMIT 1
                           ),
                           regions_embedding_search AS (
                            SELECT id, name, kind, embedding <-> @input_embedding AS distance
                            FROM vehicles_module.regions
                            WHERE 1 - (embedding <-> @input_embedding) < @max_distance
                            ORDER BY distance
                            LIMIT 1
                           )
                           SELECT 
                           cities_embedding_search.id as city_id, 
                           cities_embedding_search.name as city_name, 
                           regions_embedding_search.id as region_id, 
                           regions_embedding_search.name as region_name, 
                           regions_embedding_search.kind as region_kind 
                           FROM cities_embedding_search 
                           FULL JOIN regions_embedding_search ON true;
                           """;

        Vector vector = new(embeddings.Generate(location.Name.Value));
        DynamicParameters parameters = BuildParameters(location, vector);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgSqlSearchResult result = (await session.QuerySingleUsingReader(command, MapFromReader))!;
        List<string> locationParts = [];
        if (HasFromRegionSearch(result)) AddRegionParts(locationParts, result);
        if (HasFromCitySearch(result)) AddCityParts(locationParts, result);
        if (locationParts.Count == 0) return Error.Conflict($"Unable to resolve location from text: {location.Name}");
        if (result.RegionId == null) return Error.Conflict($"Unable to resolve region from text: {location.Name}");
        
        string locationName = string.Join(", ", locationParts);
        LocationId id = LocationId.Create(result.RegionId.Value);
        LocationName name = LocationName.Create(locationName);
        return new Location(id, name);
    }

    private static DynamicParameters BuildParameters(Location location, Vector vector)
    {
        DynamicParameters parameters = new();
        parameters.Add("@name", location.Name.Value, DbType.String);
        parameters.Add("@input_embedding", vector);
        parameters.Add("@max_distance", 0.3, DbType.Double);
        return parameters;
    }

    private static bool HasFromCitySearch(NpgSqlSearchResult result)
    {
        return result.CityId.HasValue && !string.IsNullOrWhiteSpace(result.CityName);
    }

    private static bool HasFromRegionSearch(NpgSqlSearchResult result)
    {
        return result.RegionId.HasValue &&
               !string.IsNullOrWhiteSpace(result.RegionName) &&
               !string.IsNullOrWhiteSpace(result.RegionKind);
    }

    private static void AddRegionParts(List<string> locationParts, NpgSqlSearchResult result)
    {
        locationParts.Add(result.RegionName!);
        locationParts.Add(result.RegionKind!);
    }

    private static void AddCityParts(List<string> locationParts, NpgSqlSearchResult result)
    {
        locationParts.Add(result.CityName!);
    }
    
    private static NpgSqlSearchResult MapFromReader(IDataReader reader)
    {
        Guid? cityId = reader.GetNullable<Guid>("city_id");
        string? cityName = reader.GetNullableReferenceType<string>("city_name");
        Guid? regionId = reader.GetNullable<Guid>("region_id");
        string? regionName = reader.GetNullableReferenceType<string>("region_name");
        string? regionKind = reader.GetNullableReferenceType<string>("region_kind");
        
        return new NpgSqlSearchResult
        {
            CityId = cityId,
            CityName = cityName,
            RegionId = regionId,
            RegionName = regionName,
            RegionKind = regionKind,
        };
    }
    
    private sealed class NpgSqlSearchResult
    {
        public required Guid? CityId { get; init; }
        public required string? CityName { get; init; }
        public required Guid? RegionId { get; init; }
        public required string? RegionName { get; init; }
        public required string? RegionKind { get; init; }
    }
}
    