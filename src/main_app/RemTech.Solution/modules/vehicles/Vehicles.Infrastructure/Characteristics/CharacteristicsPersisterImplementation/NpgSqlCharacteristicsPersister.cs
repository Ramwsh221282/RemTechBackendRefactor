using System.Data;
using Dapper;
using Pgvector;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using Vehicles.Domain.Characteristics;
using Vehicles.Domain.Characteristics.Contracts;

namespace Vehicles.Infrastructure.Characteristics.CharacteristicsPersisterImplementation;

public sealed class NpgSqlCharacteristicsPersister(NpgSqlSession session, EmbeddingsProvider embeddings)
	: ICharacteristicsPersister
{
	private const double MaxDistance = 0.6;

	public async Task<Result<Characteristic>> Save(Characteristic characteristic, CancellationToken ct = default)
	{
		const string sql = """
			WITH exact_match AS (
			 SELECT id, name FROM vehicles_module.characteristics WHERE name = @name
			 LIMIT 1
			),
			embedding_match AS (
			 SELECT id, name, embedding <=> @input_embedding AS distance
			 FROM vehicles_module.characteristics
			 WHERE embedding <=> @input_embedding < @max_distance
			 ORDER BY distance
			 LIMIT 1
			)
			SELECT 
			 exact_match.id as exact_id, 
			 exact_match.name as exact_name,
			 embedding_match.id as embedding_id, 
			 embedding_match.name as embedding_name
			FROM (SELECT 1) dummy
			LEFT JOIN exact_match ON true 
			LEFT JOIN embedding_match ON true;
			""";

		Vector vector = new(embeddings.Generate(characteristic.Name.Value));
		DynamicParameters parameters = BuildParameters(characteristic, vector);
		CommandDefinition command = session.FormCommand(sql, parameters, ct);
		NpgSqlSearchResult result = (await session.QuerySingleUsingReader(command, MapFromReader))!;
		if (HasFromExactSearch(result))
			return MapToCharacteristicFromExactSearch(result);
		if (HasFromEmbeddingSearch(result))
			return MapToCharacteristicFromEmbeddingSearch(result);
		await SaveAsNewCharacteristic(characteristic, vector, ct);
		return characteristic;
	}

	private static DynamicParameters BuildParameters(Characteristic characteristic, Vector vector)
	{
		DynamicParameters parameters = new();
		parameters.Add("@name", characteristic.Name.Value, DbType.String);
		parameters.Add("@input_embedding", vector);
		parameters.Add("@max_distance", MaxDistance, DbType.Double);
		return parameters;
	}

	private Task SaveAsNewCharacteristic(Characteristic characteristic, Vector vector, CancellationToken ct)
	{
		const string sql =
			"INSERT INTO vehicles_module.characteristics (id, name, embedding) VALUES (@id, @name, @input_embedding) ON CONFLICT (name) DO NOTHING";
		DynamicParameters parameters = new();
		parameters.Add("@id", characteristic.Id.Value, DbType.Guid);
		parameters.Add("@name", characteristic.Name.Value, DbType.String);
		parameters.Add("@input_embedding", vector);
		CommandDefinition command = session.FormCommand(sql, parameters, ct);
		return session.Execute(command);
	}

	private static bool HasFromEmbeddingSearch(NpgSqlSearchResult result) =>
		result.EmbeddingId.HasValue && result.EmbeddingName is not null;

	private static Characteristic MapToCharacteristicFromEmbeddingSearch(NpgSqlSearchResult result)
	{
		CharacteristicId id = CharacteristicId.Create(result.EmbeddingId!.Value);
		CharacteristicName name = CharacteristicName.Create(result.EmbeddingName!);
		return new Characteristic(id, name);
	}

	private static bool HasFromExactSearch(NpgSqlSearchResult result) =>
		result.ExactId.HasValue && result.ExactName is not null;

	private static Characteristic MapToCharacteristicFromExactSearch(NpgSqlSearchResult result)
	{
		CharacteristicId id = CharacteristicId.Create(result.ExactId!.Value);
		CharacteristicName name = CharacteristicName.Create(result.ExactName!);
		return new Characteristic(id, name);
	}

	private static NpgSqlSearchResult MapFromReader(IDataReader reader)
	{
		Guid? exactId = reader.GetNullable<Guid>("exact_id");
		string? exactName = reader.GetNullableReferenceType<string>("exact_name");
		Guid? embeddingId = reader.GetNullable<Guid>("embedding_id");
		string? embeddingName = reader.GetNullableReferenceType<string>("embedding_name");
		return new NpgSqlSearchResult
		{
			ExactId = exactId,
			ExactName = exactName,
			EmbeddingId = embeddingId,
			EmbeddingName = embeddingName,
		};
	}

	private sealed class NpgSqlSearchResult
	{
		public required Guid? ExactId { get; init; }
		public required string? ExactName { get; init; }
		public required Guid? EmbeddingId { get; init; }
		public required string? EmbeddingName { get; init; }
	}
}
